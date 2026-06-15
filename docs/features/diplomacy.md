# Diplomacy

## The Messenger System

All diplomatic communication is a **messenger unit** that physically travels across the tile map. No messages can be sent or received during the frozen window — diplomacy requires presence during the activity window.

**Movement rules:**
- Moves a fixed number of tiles per game-day tick
- Can only traverse unclaimed neutral tiles, friendly tiles, or allied tiles
- Cannot enter hostile claimed tiles — path must route around them
- Wilderness tiles beyond city range incur a loss probability per tick
- Path is calculated at send time — if the route closes after sending, the messenger may be captured or lost

## Range and City Relay

Base messenger range is limited. Range **resets and extends** each time the path passes through or adjacent to a friendly or neutral city — that city becomes a waypoint.

**Geographic power implications:**
- A city controlling a mountain pass is on every messenger route crossing those mountains — they know who is communicating and can block passage
- An isolated city is genuinely hard to reach diplomatically
- Blocking a waypoint city severs diplomatic reach for everyone behind it
- Waypoint cities know a messenger passed through

## Delivery Time

| Route Type | Approximate Delivery |
|---|---|
| Adjacent city, direct path | 5–10 ticks |
| Nearby city, 1 waypoint | 15–25 ticks |
| Distant city, 2–3 waypoints | 40–80 ticks |
| Cross-map with full relay chain | 100+ ticks |

Reply time doubles the cost. A full negotiated treaty can span multiple sessions. This limits diplomatic agreements to things players have considered carefully.

## Messenger Capture

The **intercept messenger** espionage action captures a message in transit. The sender does not know immediately whether the message was lost or intercepted.

| Action | Effect |
|---|---|
| Release | Messenger continues; capturing player gains reputation for good faith |
| Ransom | Sender pays gold to resume; small delay |
| Execute | Message never arrives; sender learns via timeout; significant reputation penalty to captor if discovered |

## Alliance Types

| Type | Structure | Notes |
|---|---|---|
| Trade League | Shared prestige bonuses, no military obligation | Breaking costs reputation |
| Defense Pact | Triggers automatically on attack, requires Authority contribution | Binding and server-enforced |
| Suzerainty | One city dominant, others semi-autonomous | Between vassal and full ally |

## Binding Treaty Mechanic

Diplomatic agreements concluded through the messenger system are **server-enforced contracts**. Breaking a treaty costs Reputation, visible to all players.

Reputation loss affects NPC merchant visit frequency and other players' willingness to negotiate.

## Discouraging Out-of-Game Communication

In-game communication is made strictly superior through:

- **Attached game state** — in-game trade offers can carry verified current gold reserves, population census, or signed treaties the server enforces automatically. Out-of-game claims cannot be verified.
- **Binding mechanic** — only in-game agreements trigger server enforcement.
- **Reputation attached to in-game messages only** — breaking an in-game treaty has mechanical consequences.
- **Permanent message log** — all in-game messages are logged and visible to both parties, serving as the authoritative record in disputes.
