# Open Design Questions

Questions raised during code review that require deliberate decisions before the relevant milestone lands. Each represents a pattern already baked into the codebase that is not described by the feature specs.

---

## 1. JWT token storage in Godot WASM

**Where does the JWT live between requests in the browser client, and is that safe?**

The server issues a 24-hour JWT on login. The client (`TokenStore` autoload) holds it in memory for the session. In a browser WASM context:

- `localStorage` persists across tabs and sessions but is readable by any XSS payload.
- `sessionStorage` is per-tab and wiped on close — users re-authenticate every tab.
- `HttpOnly` cookies (XSS-resistant) cannot be used because SignalR WebSocket connections pass the token as `?access_token=` in the URL (custom headers are not allowed on WebSocket upgrades), which cookies cannot satisfy.
- In-memory (current approach) is wiped on page refresh, requiring re-auth every reload.

There is also no token revocation mechanism. A stolen token is valid until its 24-hour expiry.

**Decision needed:** What is the acceptable token storage strategy given the WASM threat model? Options: in-memory only (simplest, re-auth on refresh), `sessionStorage` (survives navigation within tab), or a short-lived hub token separate from the API token.

---

## 2. `ActionResponse<T>` generic envelope vs. flexible delta shape

**Should `Delta` be a typed C# record, a `JsonObject`, or a `JsonElement`?**

The spec says action responses return a "typed delta — only what changed." The current implementation is `ActionResponse<T> where T : class` with a generic `Delta` property.

`System.Text.Json` serialises the declared type `T`, not the runtime type. If `T` is ever a base class or interface and a subtype is passed in, only the declared members serialise — subtype fields are silently dropped unless explicit `[JsonDerivedType]` attributes are added.

No action endpoints exist yet, so this is not a current bug. But the choice constrains how deltas can evolve.

**Decision needed:** Typed records per action (safe, explicit, more files), `JsonObject`/`JsonElement` (flexible, no polymorphism risk, no static typing), or typed with `[JsonDerivedType]` for a delta hierarchy?

---

## 3. `BackgroundService` tick vs. distributed-safe scheduler

**What happens when more than one server process runs?**

`TickService` is an in-process `BackgroundService`. If the backend ever runs with multiple replicas (availability, load), every replica runs its own tick loop — six economy phases execute N times per minute, producing duplicate results for every player.

The spec implies a single shared activity window for all players, which matches single-process deployment.

**Decision needed:** Is this a single-process deployment for the foreseeable future (keep `BackgroundService`, document the constraint)? Or does multi-instance need to be supported (requires a distributed lock, a DB-driven scheduler like Hangfire/Quartz, or a dedicated tick worker process separate from the API)?

---

## 4. Repository layer vs. raw SQL in controllers

**At what point does raw `NpgsqlDataSource` in controllers become unmanageable?**

The current pattern — `NpgsqlDataSource` injected directly into controllers, raw SQL in methods — is correct and performant. For the Foundation skeleton it's fine.

For Milestone 5 (six tick phases each touching multiple tables per city and tile per tick), and for any feature that needs the same query from multiple places, raw SQL scattered across `TickService`, `StateController`, and future action controllers becomes hard to test and hard to keep consistent.

**Decision needed:** Introduce a thin repository layer now (`CityRepository`, `TileRepository`, `PeasantRepository`) so Milestone 5 implementation doesn't have to choose between large controllers and a mid-implementation refactor?

---

## 5. In-memory `_gameTick` resets to 0 on server restart

**The game-day counter is lost on every process restart.**

`private volatile int _gameTick = 0` starts at zero each time the server starts. The spec ties real game mechanics to tick number: seasons every 120 ticks (REQ-091), harvest fires at a specific Autumn tick (REQ-092), year resets at tick 480. A restart mid-game-year — planned deploy, crash, host migration — resets all of these permanently.

**Decision needed:** Persist the current game tick (and possibly current season/year) to the database and load it at `TickService` startup? This needs to be resolved before Milestone 5 or any feature that depends on game-day counting. A `game_state` table with a single row is the simplest approach.

---

## 6. Service layer for validation logic

**Where does "can this player do this action?" logic live?**

The spec says "all authoritative state validated server-side." Currently there is no service layer — controllers query the database directly. For simple reads (state sync) this is fine. For action endpoints ("claim this tile", "set tax rate", "commission a specialist"), validation requires checking multiple pieces of state: does the player own this city, is the tile in authority range, does the city have the required buildings, does the treasury have enough gold?

Putting this in controllers leads to large, hard-to-test action methods. Duplicating it across controllers leads to divergent validation.

**Decision needed:** Do validation rules live in controller methods, in a domain service layer (`CityService.ClaimTile(...)`, `EconomyService.SetTaxRate(...)`), or in domain model methods? The earlier this is decided, the less refactoring Milestone 2 and beyond will require.

---

## 7. `HttpClient` lifetime in Godot WASM client

**The `new HttpClient()` pattern in `ActionDispatcher` is intentionally different from server-side .NET.**

On the server, `new HttpClient()` per-class is an anti-pattern: each instance holds open TCP sockets, and without `IHttpClientFactory` the OS socket limit can be exhausted under load. On Godot WASM (browser), the Mono/WASM runtime maps `HttpClient` calls to the browser's `fetch` API — the browser manages all socket lifetime, and socket exhaustion is not a risk.

`IHttpClientFactory` is not available in a Godot project and is not needed here.

**This is documented here** so future contributors familiar with server-side .NET best practices do not apply the wrong mental model to the client code.

---

## 8. `GameHub` authentication — open hub vs. authenticated hub

**Should unauthenticated clients ever be allowed to connect to the hub?**

`GameHub` now requires `[Authorize]` and the client's `WithUrl` passes an `AccessTokenProvider`. This means unauthenticated clients (no token) are rejected with 401 on WebSocket upgrade, and `WithAutomaticReconnect` retries until a token is stored via login.

The alternative — an open hub with unauthenticated connections — would allow spectator mode or a lobby. However, `Clients.All` broadcasts (currently used by `TickService` for `TickDelta`) would then reach all anonymous connections. Once tick phases are populated with per-city economy data (Milestone 5), this would leak game state to unauthenticated observers.

**Decision needed:** If spectator or lobby features are ever planned, the hub needs per-connection group management so economy deltas go only to the owning player (`Clients.User(...)` or `Clients.Group(...)`), not `Clients.All`. This should be decided before Milestone 5 populates `TickDelta` with real data.
