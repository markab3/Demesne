# Frontend

## Technology

**Godot 4 (C#) exported to Web.** See ADR-002.

Serialization: `System.Text.Json` / `JsonNode`. No Newtonsoft dependency.

## Responsibilities

The client is a display and input layer only. No authoritative game state, economy calculation, or rule enforcement lives in the client.

- Renders hex tile map and city view
- Accepts player input and sends actions to the server as HTTP POST
- Listens on SignalR connection and applies incoming deltas to local display state
- On login and reconnect, fetches full state sync via HTTP GET and rebuilds display state from it
- Sends client binary hash at authentication

## Godot Client Architecture

Two autoloaded singletons handle all state and server communication. See `docs/notes/architecture-approaches.md` for the full design session this came from.

### StateManager

Owns the local display state as a `JsonObject`. Exposes `MergePartial(JsonObject partial)` and domain-scoped Godot signals.

**Deep merge logic:**
- Nested objects → recurse
- Arrays → strategy lookup by key (see below)
- Scalars → replace only if value changed; track changed path for signals

**Array merge strategies:**

| Key | Strategy | Rationale |
|---|---|---|
| `events` | `AppendNew` | Accumulate; deduplicate by `id` |
| `units` | `MergeById` | Patch existing entries by `id` field |
| `effects` | `Replace` | Always a fresh authoritative list |

**Domain-scoped signals:**

```
TilesChanged(string[] tileKeys)   — passes only changed hex coords; enables targeted redraws
ResourcesChanged()
PopulationChanged()
TickAdvanced(int newTick)
EventsChanged()
```

`TilesChanged` passes only the coords that changed, enabling targeted tile redraws rather than full map invalidation.

### ActionDispatcher

Autoloaded singleton. Wraps all HTTP calls. On success, passes response body to `StateManager.MergePartial`. On failure, surfaces an error without mutating state.

```csharp
public async Task<bool> DispatchAsync(string endpoint, object payload)
```

Usage from any game system:
```csharp
await ActionDispatcher.DispatchAsync("actions/claim-tile", new { q, r });
await ActionDispatcher.DispatchAsync("actions/commission-specialist", new { specialistId, goodType });
```

## Display State

Client maintains local display state derived from:
1. Full state sync on login/reconnect (HTTP GET)
2. Deltas applied in ServerTimestamp order as they arrive via SignalR

Deltas arriving out of order are queued and applied in timestamp order. Display state is eventually consistent with server state. The server is always authoritative.

## Order Queue UI

During the frozen window (outside the activity window):
- Players can view current game state
- Players can submit, reorder, and cancel queued orders
- No state changes occur until the window opens
- On window open, the session open report delivers before tick 1 fires (~30–60 seconds), giving time to cancel reconsidered orders
