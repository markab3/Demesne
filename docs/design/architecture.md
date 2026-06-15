# Architecture

## Stack

| Layer | Technology |
|---|---|
| Client | Blazor WASM or Godot (C#) exported to Web |
| Backend | ASP.NET Core WebAPI |
| Real-time transport | SignalR |
| Background processing | Server-side tick processor |

Godot with C# export is the recommended client path if rendering quality is a priority — its 2D tilemap system is purpose-built for this use case. Blazor WASM is viable if a pure C# web stack is preferred, with a JavaScript canvas library (PixiJS) handling the tile map via JS interop.

## Client Role

The client is a display and input layer only. It is treated as untrusted throughout. No authoritative game state, economy calculation, or rule enforcement lives in the client. WASM binaries are decompilable — the client is a public API by definition.

## Server Authority

- All game state lives on the server and is validated there
- Client sends **intent** (claim tile, levy tax, construct building)
- Server validates against authoritative state, calculates outcomes, returns result
- Client binary hash verified server-side at authentication
- All endpoints rate limited
- No economy logic, cheat detection, or rule enforcement in the client

## Tick Processor

A background job fires once per real minute during the activity window. One tick = one game day. No ticks fire outside the activity window — the game is fully frozen.

```
Window opens
  → Session open report delivered to all players
  → ~30–60 second grace period for order review
  → Tick 1 fires
  → Queued orders execute in player-set priority
  → Economy processes (six phases — see economy-tick feature)
  → Deltas pushed to connected clients via SignalR
  → Repeat every real minute for 8 hours
Window closes — all processing stops
```

## API Communication

### Transport Split

| Transport | Direction | Usage |
|---|---|---|
| SignalR | Server → Client | Server-pushed events during active window |
| HTTP POST | Client → Server | Player-initiated actions — returns a delta |
| HTTP GET | Client → Server | Full state sync on login or reconnect |

### Action Response Envelope

```
- Success (bool)
- ErrorCode (null on success)
- ErrorMessage (null on success)
- Delta (null on failure) — sparse object, only populated fields considered changed
- ServerTimestamp — for ordering deltas client-side
- ActionId — echoes client-sent UUID for deduplication
```

Action responses return a **typed delta** — only what changed — not full state and not a bare success/fail. A periodic full state sync corrects accumulated drift on reconnect or login.

### Fog of War

Full state sync returns only tiles within the player's visibility range. Sending full map data to every client is a performance problem and a security problem.

## Security Model

- All authoritative game state validated server-side
- Client binary hash verified at authentication (hash computed at build time, stored server-side)
- All endpoints rate limited
- No secret business logic in the client
- WASM treated as public API at all times
