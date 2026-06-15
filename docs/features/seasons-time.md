# Seasons & Time

## Time Scale

| Unit | Real Time |
|---|---|
| 1 game day | 1 real minute |
| 1 server tick | 1 game day |
| 1 game year | 8 real hours (480 ticks) |
| 1 season | 2 real hours (120 ticks) |

## Seasonal Structure

| Season | Game Days | Real Time | Agricultural Effect |
|---|---|---|---|
| Spring | Days 1–120 | Hours 1–2 | Planting — corvée levy incurs yield penalty |
| Summer | Days 121–240 | Hours 3–4 | Growth — steady partial yield, pasture peak |
| Autumn | Days 241–360 | Hours 5–6 | Harvest — primary crop yield fires, corvée risk |
| Winter | Days 361–480 | Hours 7–8 | Fallow — no crop yield, food consumed, ideal corvée window |

## Harvest Mechanic

Crops accumulate in a field progress meter per tile during Spring and Summer. At a specific predictable Autumn tick — the same real-world clock time every session — the harvest fires and converts accumulated progress to stored goods in one transaction.

**Harvest size depends on:**
- Arable percentage
- Tool quality (tools must be in peasant tile inventory)
- Peasant population
- Weather events during Summer
- Whether corvée was levied during planting season

The harvest tick is the session's primary economic event. Players can schedule to be present for it.

## Activity Window

The game runs during a shared 8-hour window per day. Outside the window the game is **fully frozen** — no ticks are processed, no military movement occurs.

- All players in a game instance share the same window
- Window timing is advertised at instance join
- Players self-select into instances matching their availability
- Warfare requires presence during the window — a player cannot be overrun while offline

## Queued Orders

Orders submitted during the frozen window execute at window open in player-set priority order. Players can reorder, modify, or cancel queued orders freely while frozen. Failed orders report cleanly with a reason at session open.

## Session Open Report

When the window opens, each player receives:
- Current stores of all goods
- Authority budget and projected daily balance
- Queued orders pending execution in priority order
- Diplomatic messages received during the frozen window
- Visible map changes from the previous session

The first tick fires ~30–60 real seconds after window open, giving players time to cancel reconsidered orders.
