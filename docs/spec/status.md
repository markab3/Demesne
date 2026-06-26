# Implementation Status

## Foundation

Prerequisites before feature work can begin. Driven by `docs/design/` rather than feature specs.

### Infrastructure

- [x] ASP.NET Core WebAPI project scaffolded, runs locally (`dotnet run`)
- [x] Database schema initialized from `docs/design/data-model.md`
- [x] SignalR hub configured; client can connect and receive a server push
- [ ] Godot 4 (C#) project scaffolded, runs in-editor against local API
- [ ] `StateManager` autoload: holds display state, exposes `MergePartial`, emits domain-scoped signals
- [ ] `ActionDispatcher` autoload: wraps all HTTP POST calls with typed methods
- [ ] Player auth: register, login, session token issued and validated on subsequent requests
- [ ] Full-state sync on login/reconnect
- [ ] Delta response shape: action responses return sparse changed-field objects (REQ-205)
- [ ] Tick processor skeleton: fires once per real minute during activity window, six empty phase stubs in order

### Deferred — Hosting & Export

- [ ] Godot project exported to Web (WASM build)
- [ ] ASP.NET backend deployed to a hosted environment

---

## Milestones

Milestones organize REQ implementation into deliverable slices. Each builds on the previous. Remaining Foundation items above are prerequisites for Milestone 1.

### Milestone 1 — Walking Skeleton
*Thin vertical slice through every layer. Nothing playable — proves the full stack works end-to-end.*

- [ ] Single tile endpoint: GET returns terrain fields for one hardcoded tile (REQ-001 partial)
- [ ] Godot client fetches and displays that tile's terrain fields as text
- [ ] Tick fires once/minute and increments a game-day counter (REQ-090 partial)
- [ ] SignalR pushes the day counter delta to all connected clients (REQ-205 partial)

### Milestone 2 — First Production Loop
*A peasant tile produces something visible each tick. First economic feedback.*

- [ ] Peasant record on a tile: population count and happiness (REQ-020)
- [ ] Phase 1 production calculates output for that tile (REQ-011 partial)
- [ ] Phase 2 goods tax deducted to city stockpile (REQ-021, REQ-051 partial)
- [ ] Client displays peasant output and stockpile contents, updating each tick

### Milestone 3 — Player Identity & Multi-City
*Multiple players can connect with separate city state. Auth and sync wired.*

- [ ] Player auth complete: register, login, session token validated (Foundation item)
- [ ] Full-state sync on login/reconnect (Foundation item)
- [ ] Delta response shape enforced on all action responses (REQ-205)
- [ ] Fog of war: full sync returns only tiles in visibility range (REQ-204)
- [ ] Two independent cities can be created and viewed by separate players

### Milestone 4 — Map Generation
*Procedural map replaces the hardcoded test tile.*

- [ ] Simplex noise generation: altitude, temperature, precipitation, arable % per tile (REQ-001)
- [ ] Forest tiles with near-zero arable % (REQ-002 partial)
- [ ] Mineral deposit quantities vary per tile (REQ-003)
- [ ] Hex grid with axial coordinates queryable from client (ADR-005)

### Milestone 5 — Full Economy Tick
*All six phases running with real logic. First genuinely playable economic loop.*

- [ ] All six tick phases executing in order with real logic (REQ-011)
- [ ] Prestige as continuous output rate (REQ-013)
- [ ] Authority income and deficit cascade (REQ-014)
- [ ] Happiness drivers wired (REQ-015)
- [ ] Crime pool mechanics (REQ-016)
- [ ] Tile upkeep scaling formula (REQ-017)

### Milestone 6 — Specialists
- [ ] REQ-030 through REQ-037 (see Requirements table)

### Milestone 7 — Retainers
- [ ] REQ-040 through REQ-044 (see Requirements table)

### Milestone 8 — Stockpile & Merchants
- [ ] REQ-050 through REQ-054, REQ-080 through REQ-085 (see Requirements table)

### Milestone 9 — Guilds & Education
- [ ] REQ-060 through REQ-062, REQ-070 through REQ-072 (see Requirements table)

### Milestone 10 — Seasons & Time
- [ ] REQ-090 through REQ-094 (see Requirements table)

### Milestone 11 — Military & Diplomacy
- [ ] REQ-100 through REQ-104, REQ-110 through REQ-115 (see Requirements table)

### Milestone 12 — Vassals & Manors
- [ ] REQ-120 through REQ-122, REQ-130 through REQ-133 (see Requirements table)

### Milestone 13 — Hardening
*Non-functional requirements and security posture.*

- [ ] REQ-200 through REQ-206 (see Requirements table)

---

## Requirements

| REQ | Description | Status |
|---|---|---|
| **World** | | |
| REQ-001 | Map gen: altitude, temperature, precipitation, arable % per tile | pending |
| REQ-002 | Forest clearing: raises arable %, removes timber resource permanently | pending |
| REQ-003 | Mineral deposit quantity varies per tile; no rock type | pending |
| REQ-004 | No two cities with identical natural specializations (probabilistic) | pending |
| **Economy — Core** | | |
| REQ-010 | Three pop tiers: Peasants (aggregate/tile), Specialists (named), Retainers (named) | pending |
| REQ-011 | Six-phase tick in order: Production → Taxes & Salary → Consumption → List for Sale → Retainer Requisition → Purchases | pending |
| REQ-012 | Goods must be in pop's inventory to have effect; stockpile goods inert | pending |
| REQ-013 | Prestige: continuous linear rate; sets trade range, diplomatic weight, specialist attraction | pending |
| REQ-014 | Authority: income = population × happiness modifier; deficit cascades | pending |
| REQ-015 | Happiness drivers (positive and negative) affect authority production | pending |
| REQ-016 | Crime: pool metric, accumulates/decays; threshold effects on gold, happiness, buildings | pending |
| REQ-017 | Tile upkeep: linear 1–10, ×1.5 at 11–20, ×2.5 at 21+ | pending |
| **Labor — Peasants** | | |
| REQ-020 | Peasants: aggregate count + collective happiness per tile | pending |
| REQ-021 | Goods tax: player-set %, applies only to goods the tile actually produces | pending |
| REQ-022 | Corvée: fixed annual worker-days per tile; church auto-claim non-redirectable | pending |
| REQ-023 | Corvée during planting/harvest reduces goods tax yield that cycle | pending |
| REQ-024 | Unhappiness escalation: petty crime → crime+authority drain → banditry → revolt | pending |
| **Labor — Specialists** | | |
| REQ-030 | Specialist record fields: Name, PrimaryTrade, SkillRating, SecondarySkills, Inventory, Gold, Happiness, CommissionCapacity | pending |
| REQ-031 | Specialists pay goods tax on finished output, not raw inputs | pending |
| REQ-032 | Specialists operate autonomously when no commission is active | pending |
| REQ-033 | Commission capacity typically 20–40%; emergency full requisition available with loyalty penalty | pending |
| REQ-034 | Specialist departure: warning → winding down → departure; rival may recruit departing specialist | pending |
| REQ-035 | Craft Rating: continuous 1–100 per industry; climbs with employment, decays with departures | pending |
| REQ-036 | Specialist tiers within trade: Apprentice, Journeyman, Master | pending |
| REQ-037 | Specialist attraction: threshold conditions trigger named candidate; player approves or declines | pending |
| **Labor — Retainers** | | |
| REQ-040 | Retainers idle until assigned an order | pending |
| REQ-041 | Retainer types: Professional Soldier, General, Knight, Spy, Diplomat, Herald | pending |
| REQ-042 | Salary deducted each tick; missed payments escalate to desertion and defection | pending |
| REQ-043 | Knights and Generals require mandatory luxury consumption | pending |
| REQ-044 | Equipment degrades per tick; replenishment from stockpile restores effectiveness | pending |
| **City Stockpile** | | |
| REQ-050 | Stockpile is lord's personal inventory, distinct from city market | pending |
| REQ-051 | Stockpile sources: goods tax (Phase 2), commissioned output, lord purchases (Phase 6), vassal tribute | pending |
| REQ-052 | Buy parameters per good: minimum threshold, maximum ceiling, maximum price | pending |
| REQ-053 | Goods above ceiling listed on market at end of Phase 3 | pending |
| REQ-054 | Siege cuts merchant access; stockpile depth determines holding time | pending |
| **Guild System** | | |
| REQ-060 | Guild mediates lord–craftsmen; player interacts with guild as collective | pending |
| REQ-061 | Guild hall accelerates Craft Rating and trains apprentices | pending |
| REQ-062 | Guild standing governs available commission capacity and price | pending |
| **Education** | | |
| REQ-070 | Literacy prerequisite for secondary skills; unlocks via guild training otherwise | pending |
| REQ-071 | Books increase Craft Rating growth rate | pending |
| REQ-072 | Education buildings: Scriptorium, Guild Hall, Apothecary School, Military Academy, Counting House, University | pending |
| **Merchant System** | | |
| REQ-080 | Merchants physically travel between cities; tick participation at current city | pending |
| REQ-081 | Merchants in foreign cities: Phase 4 and Phase 6 only | pending |
| REQ-082 | Trade ledger: personally observed prices, entries age, shared with guild members in same city | pending |
| REQ-083 | Routing follows standing instructions set at departure; no mid-journey recalculation | pending |
| REQ-084 | Merchant travel range bounded by home city prestige | pending |
| REQ-085 | Host city market fee; home city charter fee; staple rights at chokepoints | pending |
| **Seasons & Time** | | |
| REQ-090 | 1 real minute = 1 game day = 1 server tick (during activity window) | pending |
| REQ-091 | 1 game year = 480 game days = 8 real hours = 4 seasons of 120 days each | pending |
| REQ-092 | Harvest fires at specific Autumn tick; size depends on arable %, tools, population, weather, corvée timing | pending |
| REQ-093 | Frozen window orders queue and execute at window open in priority order | pending |
| REQ-094 | Session open report delivered to all players at window open | pending |
| **Military** | | |
| REQ-100 | Force projection outside territory requires a General; upkeep is Authority | pending |
| REQ-101 | Generals have loyalty ratings; can defect with army | pending |
| REQ-102 | General command radius; armies beyond it suffer attrition | pending |
| REQ-103 | Sieges take multiple game days; fortification slows progress; allies can send relief | pending |
| REQ-104 | No military movement or tile claiming outside the activity window | pending |
| **Diplomacy** | | |
| REQ-110 | Diplomatic communication via physical messenger units on the tile map | pending |
| REQ-111 | Messengers: fixed tiles/tick movement, cannot enter hostile tiles, path fixed at send time | pending |
| REQ-112 | Messenger range resets at friendly/neutral city waypoints | pending |
| REQ-113 | Waypoint cities notified of transit; messengers can be intercepted | pending |
| REQ-114 | Treaty types: Trade League, Defense Pact, Suzerainty; breaking costs Reputation | pending |
| REQ-115 | In-game messages can carry verified game state | pending |
| **Vassal System** | | |
| REQ-120 | Defeated city vassalized; keeps playing; attacker receives tribute and prestige | pending |
| REQ-121 | Vassals can be liberated by allies or buy independence | pending |
| REQ-122 | Over-vassalizing drains Authority rapidly | pending |
| **Manor System** | | |
| REQ-130 | Each inhabited tile administered through a manor held by a noble house | pending |
| REQ-131 | Nobles cut peasant output before city tax; deliver feudal levy per season | pending |
| REQ-132 | Noble loyalty affects tax pass-through and military reliability; low loyalty enables defection | pending |
| REQ-133 | Accountant specialist reduces noble skim regardless of loyalty | pending |
| **Non-Functional** | | |
| REQ-200 | Server-authoritative: all state and validation server-side | pending |
| REQ-201 | No economy logic in client binary | pending |
| REQ-202 | Client binary hash verified server-side at authentication | pending |
| REQ-203 | All endpoints rate limited | pending |
| REQ-204 | Fog of war enforced server-side | pending |
| REQ-205 | Action responses return typed delta only; full sync on reconnect | pending |
| REQ-206 | No game logic derivable from WASM decompilation | pending |
