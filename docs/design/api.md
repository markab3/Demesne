# API

## Transport Summary

| Transport | Direction | Usage |
|---|---|---|
| SignalR | Server → Client | Server-pushed events during active window |
| HTTP POST | Client → Server | Player-initiated actions |
| HTTP GET | Client → Server | Full state sync on login or reconnect |

## Action Response Envelope

All HTTP POST action responses use this envelope:

```json
{
  "success": bool,
  "errorCode": "string | null",
  "errorMessage": "string | null",
  "delta": "object | null",
  "serverTimestamp": int,
  "actionId": "string"
}
```

- `delta` is a sparse object — only populated fields are considered changed. Null on failure.
- `serverTimestamp` allows client-side ordering of deltas.
- `actionId` echoes the client-sent UUID for deduplication.

## Action Categories (HTTP POST)

- **Tile actions:** claim tile, build improvement, levy corvée
- **City actions:** set tax rate, construct building, commission specialist, adjust buy orders
- **Military actions:** raise/dismiss units, issue general orders, initiate siege
- **Diplomatic actions:** send messenger, respond to offer, ratify/break treaty
- **Retainer actions:** assign mission (spy, diplomat, herald)
- **Order queue actions:** submit, reorder, cancel queued orders

## State Sync (HTTP GET)

Full state sync returns:
- All city data (city, stockpile, market, buildings, pops)
- All claimed tiles within authority range
- All visible map tiles (fog of war applied — only tiles within visibility range)
- Active diplomatic agreements
- Queued orders with current priority

Full sync corrects accumulated delta drift. Issued on login and reconnect.

## SignalR Event Types

Server pushes events per tick during the activity window:

| Event | Trigger |
|---|---|
| TickDelta | Every tick — economy state changes for owned pops and tiles |
| TileChanged | A visible tile's controller, terrain, or content changed |
| ArmyApproaching | Enemy army enters visibility range |
| MessageDelivered | Messenger arrived |
| MessageCaptured | Messenger captured in transit |
| SpecialistDeparting | Specialist loyalty below threshold, warning fired |
| SessionOpenReport | Window opens — full summary delivered |
| OrderFailed | Queued order failed at execution; includes reason |
| TreatyBroken | A party broke an in-game treaty; reputation cost applied |

## Fog of War Constraint

The server never sends tile data outside the player's current visibility range. Visibility is determined by:
- Claimed tiles (always visible)
- Adjacent unclaimed tiles within prestige range
- Scout and cartographer range extensions

A player cannot receive economic or military intelligence about tiles they cannot see.
