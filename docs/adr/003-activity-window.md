# ADR-003: Frozen Activity Window Model

Status: Decided

## Context

A persistent multiplayer world creates problems for players with different schedules. Real-time games punish offline players with permanent territory loss. Fully asynchronous games lose tension and meaningful interaction. The game needs to handle both the social coordination problem and the griefing problem.

## Decision

The game runs during a shared 8-hour activity window per day. Outside this window the game is fully frozen — no ticks, no military movement, no territory changes. Players in the same instance share the same window. Instance timing is advertised at join; players self-select into instances matching their availability.

## Consequences

- Warfare is structurally impossible against an offline player — no separate protection mechanic needed
- Players in the same instance must have compatible availability windows — this constrains matchmaking
- The harvest tick (the session's primary economic event) fires at a predictable real-world time each session — players can schedule for it
- Queued orders let players prepare during the frozen window and execute on open
- The game does not require 24/7 attention; meaningful decisions happen within the 8-hour window
- Diplomacy is also frozen outside the window — no messenger movement, no surprise treaties
