# Godot 4 + ASP.NET Core Implementation Patterns

Source: design session exploring Godot client architecture and server communication patterns.

---

## Stack

| Layer | Technology |
|---|---|
| Game client | Godot 4, C#, exported to Web |
| Backend | ASP.NET Core WebAPI |
| Serialization | `System.Text.Json` / `JsonNode` |
| Transport | HTTPS, `application/json` |

---

## Partial Response Pattern

Each action endpoint:
1. Receives action parameters (tile coords, structure type, etc.)
2. Validates and computes the resulting changes against authoritative server state
3. Returns **only the slice of state that changed** — not the full state

The client display state merges each partial response:

```
Godot Client                        ASP.NET Backend
─────────────────────────           ──────────────────────────
Display state (server-derived)      Authoritative game state + logic
      │                                      │
      │  POST /actions/build-structure        │
      │  { tileX, tileY, structureType } ───►│ Validate + compute changes
      │                                       │
      │  ◄── 200 OK { partial delta } ───────│ Return ONLY what changed
      │
 Merge delta into display state
 Emit signals for changed paths
 UI/systems react
```

---

## Patching Standard: Custom Deep Merge

Two RFC standards evaluated and rejected:

**JSON Patch (RFC 6902)** — explicit operations on specific paths:
```json
[
  { "op": "replace", "path": "/player/health", "value": 80 },
  { "op": "add", "path": "/inventory/-", "value": { "id": "sword_01" } }
]
```

**JSON Merge Patch (RFC 7396)** — send only changed fields; `null` means remove:
```json
{ "player": { "health": 80 }, "currentZone": null }
```

**Decision:** Neither RFC adopted. Instead:
- Server returns plain JSON objects containing only the changed subtrees
- Client performs a **recursive deep merge** of the partial response into its display state
- Simpler for the server to produce than JSON Patch; more expressive than JSON Merge Patch for array handling
- No JSON Patch NuGet (`Microsoft.AspNetCore.JsonPatch`, `Morcatko.AspNetCore.JsonMergePatch`) — avoids Newtonsoft dependency; merge logic lives in the Godot client using `System.Text.Json.Nodes.JsonObject`

---

## State Shape Principles

Key structural decisions for the state sync payload and delta responses:

- **Tiles as dict keyed by hex coordinate string** (e.g. `"q,r"`) — server returns only changed tile coordinates, not the full map
- **Cities and units with stable IDs** — enables merge-by-id without position ambiguity
- **Domain nodes as separate top-level keys** — UI subsystems subscribe only to the paths they care about
- **Events as append-only array** — new events accumulate; deduplication by `id` field

These principles apply to both the full state sync (HTTP GET on login/reconnect) and partial delta responses (HTTP POST action results).

Actual field names and shape follow the data model in `docs/design/data-model.md`.

---

## Godot Client Architecture

### StateManager (Autoloaded Singleton)

Owns the client's display copy of server state as a `JsonObject`. Exposes `MergePartial(JsonObject delta)` and domain-scoped Godot signals.

**Deep Merge Logic:**
- Nested objects → recurse
- Arrays → strategy lookup by key (below)
- Scalars → replace only if value actually changed; track changed path for signal emission

**Array Merge Strategies:**

| Key | Strategy | Rationale |
|---|---|---|
| `events` | `AppendNew` | Accumulate; deduplicate by `id` |
| `units` | `MergeById` | Patch existing entries by `id` field |
| `effects` | `Replace` | Always a fresh authoritative list |

**Signals (domain-scoped):**
```
TilesChanged(string[] tileKeys)   — passes only changed hex coords; enables targeted tile redraws
ResourcesChanged()
PopulationChanged()
TickAdvanced(int newTick)
EventsChanged()
```

`TilesChanged` passes only the coords that changed, enabling targeted tile redraws rather than full map invalidation.

### ActionDispatcher (Autoloaded Singleton)

Wraps all HTTP calls. On success, passes response body to `StateManager.MergePartial`. On failure, surfaces an error without mutating display state.

```csharp
public async Task<bool> DispatchAsync(string endpoint, object payload)
```

Usage from any game system:
```csharp
await ActionDispatcher.DispatchAsync("actions/claim-tile", new { q, r });
await ActionDispatcher.DispatchAsync("actions/commission-specialist", new { specialistId, goodType });
```

---

## Backend Action Endpoint Pattern

Each endpoint is discrete and returns only the state it touches:

```jsonc
// POST /actions/claim-tile
{ "tiles": { "4,7": { "controller": "city_01", "upkeepAdded": 12 } },
  "authority": { "budget": -12, "projectedDaily": -8 } }

// POST /actions/levy-tax
{ "stockpile": { "grain": 240, "wool": 80 },
  "tiles": { "2,3": { "peasantHappiness": 72 } } }

// POST /actions/commission-specialist
{ "specialists": { "spec_14": { "commissionCapacity": 0.35, "commissionGood": "iron_tools" } },
  "treasury": { "gold": 1180 } }
```
