# ADR-001: Server-Authoritative Economy

Status: Decided

## Context

The game is multiplayer. Players share a world with territorial control, inter-city trade, diplomacy, and military conflict — all of which require a single source of truth. A client-authoritative or hybrid model risks exploitation via client manipulation and makes anti-cheat enforcement unreliable. WASM binaries are decompilable — anything in the client is public.

## Decision

All authoritative game state lives on the server and is validated there. The client is treated as a public API throughout. No economy logic, cheat detection, or rule enforcement lives in the client.

## Consequences

- Client sends intent; server validates against authoritative state and returns a typed delta
- Client binary hash verified server-side at authentication
- All endpoints rate limited
- Full state sync on login/reconnect handles any client drift
- Fog of war enforced server-side — only tiles within visibility range sent to each client
- Economy calculations (all six tick phases) are server-exclusive
