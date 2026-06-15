# ADR-004: Typed Delta API Responses

Status: Decided

## Context

A tick-based economy where many pops change state every minute creates significant data volume per response. Three options: (1) return full state on every action, (2) return success/fail only and let clients poll, (3) return a typed sparse delta of only what changed.

## Decision

Action responses return a typed delta — only populated fields are considered changed. SignalR pushes deltas per tick. A full state sync on login/reconnect handles initial load and recovery from disconnection.

## Consequences

- Reduced payload size per response compared to full state
- Client must apply deltas in ServerTimestamp order to maintain consistency
- ActionId echoed in each response enables client-side deduplication
- Full state sync required on login and reconnect — delta-only is not sufficient for initial load
- Fog of war enforcement is implicit: delta only includes tiles within the player's visibility range
