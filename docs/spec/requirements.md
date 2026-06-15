# Requirements

## Functional Requirements

### World

REQ-001: The map is generated procedurally with terrain parameters per tile: altitude, temperature, precipitation, and arable percentage.
REQ-002: Forest tiles have near-zero arable percentage. Clearing a forest tile permanently raises arable percentage and removes the timber resource.
REQ-003: Mineral deposits vary in quantity per tile. Rock type is not modeled.
REQ-004: No two cities begin with identical natural specializations due to asymmetric terrain generation.

### Economy — Core

REQ-010: Three distinct pop tiers exist: Peasants (aggregate per tile), Specialists (named individual records), and Retainers (named individual records).
REQ-011: Every game-day tick processes all pops through six phases in order: Production, Taxes & Salary, Consumption, List for Sale, Retainer Requisition, Purchases.
REQ-012: Goods must be physically present in a pop's inventory to have effect. Goods in the stockpile do not passively benefit other pops.
REQ-013: Prestige is a continuous linear output metric (a rate, not a tier). It directly and continuously sets trade range radius, diplomatic weight, and specialist attraction — no thresholds or gates.
REQ-014: Authority governs territorial control and military projection. Authority income is derived from population × happiness modifier. Deficit triggers a cascade of territorial and civil consequences.
REQ-015: Happiness drives authority production. Positive drivers: food variety, luxury goods access, trade activity, cultural buildings. Negative drivers: crime, tax rate, recent military losses, food shortage.
REQ-016: Crime is a pool metric that accumulates and decays. Above threshold: gold drain and happiness penalty. At critical levels: specific buildings become temporarily non-functional.
REQ-017: Tile upkeep scales exponentially beyond 20 tiles (tiles 1–10 linear, 11–20 at ×1.5, 21+ at ×2.5), making large empires structurally fragile.

### Labor — Peasants

REQ-020: Peasants are tracked as an aggregate population count per tile with a single collective happiness score.
REQ-021: Goods tax is a player-set percentage of peasant output, applicable only to goods that tile's population actually produces.
REQ-022: Corvée labor is a fixed annual obligation in worker-days per tile, used for major construction. The church automatically claims a portion that cannot be redirected.
REQ-023: Levying corvée during planting or harvest seasons reduces goods tax yield for that cycle.
REQ-024: Peasant unhappiness escalates: petty crime → significant crime and authority drain → banditry and hidden output → revolt (NPC army, tile control contested).

### Labor — Specialists

REQ-030: Each specialist is a named individual record with a primary trade at a skill rating, optional secondary skills, a personal inventory, and an individual happiness value.
REQ-031: Specialists pay goods tax on finished output, not raw inputs.
REQ-032: Specialists operate private businesses. The lord taxes, commissions, and provides legal and physical framework. The market mediates all transactions.
REQ-033: Each specialist has a commission capacity (typically 20–40% of working time) the lord can direct. Emergency full requisition is available but damages loyalty if used repeatedly.
REQ-034: Specialists can depart when loyalty drops: warning → winding down → departure. A rival city with a spy or herald in range may intercept and recruit a departing specialist.
REQ-035: Craft Rating is a continuous 1–100 scale per industry. It climbs with sustained specialist employment and relevant guild buildings. It decays if specialists leave or buildings fall into disrepair.
REQ-036: Specialists exist in three tiers within each trade: Apprentice, Journeyman, Master. Masters are scarce enough that losing one to a rival is a meaningful strategic event.
REQ-037: Specialists are attracted by conditions crossing a threshold, at which point a named candidate arrives with a starting skill level and personality traits. The player approves or declines.

### Labor — Retainers

REQ-040: Retainers are salaried named individuals who wait for orders and do nothing until assigned.
REQ-041: Retainer types: Professional Soldier, General, Knight, Spy, Diplomat, Herald.
REQ-042: Retainer salary is deducted from the city treasury every tick. Missed payments escalate: morale warning → effectiveness penalty and desertion → mass desertion and potential defection with forces.
REQ-043: Knights and Generals have mandatory luxury consumption as part of their upkeep, not optional.
REQ-044: Equipment degrades over ticks and must be replenished from the stockpile to maintain effectiveness.

### City Stockpile

REQ-050: The city stockpile is the lord's personal inventory, distinct from the city market. Goods in the stockpile do not participate in market phases automatically.
REQ-051: Stockpile sources: goods tax from peasant tiles (Phase 2), commissioned output, lord purchases (Phase 6), vassal tribute.
REQ-052: The lord sets per-good buy parameters: minimum threshold, maximum ceiling, maximum price. Unmet orders do not create debt.
REQ-053: Goods above the stockpile maximum ceiling are listed on the city market at the end of Phase 3.
REQ-054: During a siege, merchant access is cut off. Stockpile depth at siege onset determines how long the city holds before retainer effectiveness degrades.

### Guild System

REQ-060: The guild mediates between craftsmen and the lord per trade. The player interacts with the guild as a collective, not individual specialists.
REQ-061: The guild hall accelerates Craft Rating and trains apprentices. Without it, specialists operate individually and are harder to direct and easier to lose one by one.
REQ-062: Guild standing determines available commission capacity and price. Good standing enables self-regulation of apprentice training and quality.

### Education

REQ-070: Literacy is a prerequisite for secondary skills (accounting, alchemy, tactics). Specialists without literacy advance only through guild training, which is slower.
REQ-071: Books increase Craft Rating growth rate. A city with a scriptorium and steady book supply trains specialists measurably faster.
REQ-072: Education buildings: Scriptorium, Guild Hall, Apothecary School, Military Academy, Counting House, University.

### Merchant System

REQ-080: Merchants are named specialist records that physically travel between cities and participate in the economy tick of whichever city they are currently in.
REQ-081: Merchants participate in Phase 4 (list carried goods) and Phase 6 (purchase goods to carry out) only. No Phase 1 production, no Phase 5 stockpile access in foreign cities.
REQ-082: Each merchant maintains a trade ledger of personally observed prices. Ledger entries age. Merchants share ledger entries with guild members in the same city.
REQ-083: Merchant routing follows standing instructions set at departure. No mid-journey recalculation.
REQ-084: Merchant travel range is bounded by home city prestige.
REQ-085: Host cities collect a market fee on foreign merchant sales. Home city collects a periodic guild charter fee. Cities controlling strategic chokepoints can assert staple rights.

### Seasons & Time

REQ-090: 1 real minute = 1 game day = 1 server tick (during the activity window).
REQ-091: 1 game year = 480 game days = 8 real hours = 4 seasons of 120 days each.
REQ-092: The harvest fires at a specific predictable Autumn tick. Harvest size depends on arable %, tool quality (in peasant tile inventory), peasant population, summer weather events, and corvée timing.
REQ-093: Orders submitted during the frozen window execute at window open in player-set priority order. Failed orders report with a reason.
REQ-094: Session open report delivered to all players at window open: current stores, authority budget, queued orders, diplomatic messages received, visible map changes.

### Military

REQ-100: Projecting military force outside territory requires a General. Generals cost Authority upkeep, not gold.
REQ-101: Generals have loyalty ratings and can defect, taking their army with them.
REQ-102: Each general has a command radius; armies beyond it suffer attrition.
REQ-103: Sieges take multiple in-game days. Fortification ratings slow progress. Allies can send relief during an ongoing siege.
REQ-104: No military movement or tile claiming occurs outside the activity window.

### Diplomacy

REQ-110: All diplomatic communication is a messenger unit that physically travels across the tile map.
REQ-111: Messengers move a fixed number of tiles per tick. They cannot enter hostile claimed tiles. Path is calculated at send time.
REQ-112: Messenger range resets and extends each time the path passes through or adjacent to a friendly or neutral city (waypoint).
REQ-113: Waypoint cities know a messenger passed through. Messengers can be intercepted via espionage: released, ransomed, or executed.
REQ-114: Server-enforced treaty types: Trade League, Defense Pact, Suzerainty. Breaking an in-game treaty costs Reputation, visible to all players.
REQ-115: In-game messages can carry verified game state (gold reserves, population census, signed treaties) that out-of-game communication cannot provide.

### Vassal System

REQ-120: When a defeated city is vassalized it keeps its city and continues playing. The attacker receives tribute (percentage of trade output) and prestige from dominion.
REQ-121: Vassals can be liberated by allied forces or buy independence with gold.
REQ-122: Over-vassalizing drains Authority rapidly — conquest is a calculated economic decision.

### Manor System

REQ-130: Each inhabited tile is administered through a manor held by a noble house.
REQ-131: Nobles take a cut of peasant output before the city tax share, in exchange for local administration and a fixed feudal levy per season.
REQ-132: Noble loyalty determines how much tax passes through and whether the military obligation is delivered reliably. Low-loyalty nobles may defect.
REQ-133: An accountant specialist reduces the amount nobles can skim regardless of loyalty.

## Non-Functional Requirements

REQ-200: All authoritative game state lives on the server and is validated there. The client is treated as untrusted throughout.
REQ-201: No secret business logic in the client — economy calculations, cheat detection, and rule enforcement are server-exclusive.
REQ-202: Client binary hash is verified server-side at authentication.
REQ-203: All endpoints are rate limited.
REQ-204: Fog of war enforced server-side — full state sync returns only tiles within the player's current visibility range.
REQ-205: Action responses return a typed delta (only changed fields) rather than full state. Full state sync corrects drift on reconnect.
REQ-206: WASM is treated as a public API. No game logic should be derivable from client-side decompilation.
