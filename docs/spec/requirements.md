# Requirements

Testability criteria follow each requirement. Requirements marked *Non-testable as stated* are design properties or postures verifiable only by code review or inspection, not by a discrete pass/fail test.

## Functional Requirements

### World

REQ-001: The map is generated procedurally with terrain parameters per tile: altitude, temperature, precipitation, and arable percentage.
  *Testable when:* every tile in a generated map returns valid non-null values for all four parameters; querying any tile's schema confirms all four fields are present.

REQ-002: Forest tiles have near-zero arable percentage. Clearing a forest tile permanently raises arable percentage and removes the timber resource.
  *Testable when:* (a) a forest tile's ArablePercentage increases after a clear action is processed; (b) the Timber resource entry is absent from that tile's resource list post-clear; (c) no server action exists to reverse the change.

REQ-003: Mineral deposits vary in quantity per tile. Rock type is not modeled.
  *Testable when:* two mineral tiles of the same type show different deposit quantities across a sample of generated maps; the tile schema has no rock-type field.

REQ-004: No two cities begin with identical natural specializations due to asymmetric terrain generation.
  *Non-testable as stated:* this is a probabilistic design property of the generator, not a deterministic guarantee. Verifiable by inspection of starting positions across a large sample of generated maps, not by a discrete pass/fail test.

### Economy — Core

REQ-010: Three distinct pop tiers exist: Peasants (aggregate per tile), Specialists (named individual records), and Retainers (named individual records).
  *Testable when:* querying a city's pop records returns Peasant records with no name field (aggregate per tile), and Specialist and Retainer records each with a unique name field — each type with a distinct schema.

REQ-011: Every game-day tick processes all pops through six phases in order: Production, Taxes & Salary, Consumption, List for Sale, Retainer Requisition, Purchases.
  *Testable when:* tick execution logs confirm phases fire in the specified order with no phase skipped or transposed, across all pops.

REQ-012: Goods must be physically present in a pop's inventory to have effect. Goods in the stockpile do not passively benefit other pops.
  *Testable when:* (a) iron tools in a peasant tile's inventory produce a higher yield than an identical tile without tools; (b) the same tools sitting in the city stockpile produce no yield change on any peasant tile.

REQ-013: Prestige is a continuous linear output metric (a rate, not a tier). It directly and continuously sets trade range radius, diplomatic weight, and specialist attraction — no thresholds or gates.
  *Testable when:* incrementing prestige by 1 produces a proportional change in trade range, diplomatic weight, and attraction with no step function or threshold gate detectable in the calculation.

REQ-014: Authority governs territorial control and military projection. Authority income is derived from population × happiness modifier. Deficit triggers a cascade of territorial and civil consequences.
  *Testable when:* (a) authority income at 100% happiness equals base_population × 1.0; (b) at 50% happiness equals approximately 0.5 × base_population; (c) sustained negative budget triggers cascade consequences in the specified priority order.

REQ-015: Happiness drives authority production. Positive drivers: food variety, luxury goods access, trade activity, cultural buildings. Negative drivers: crime, tax rate, recent military losses, food shortage.
  *Testable when:* each driver is toggled in isolation in a test environment and produces a measurable change in happiness in the correct direction.

REQ-016: Crime is a pool metric that accumulates and decays. Above threshold: gold drain and happiness penalty. At critical levels: specific buildings become temporarily non-functional.
  *Testable when:* (a) sustained authority deficit increases the crime pool value each tick; (b) restoring a positive budget decreases it; (c) at the defined critical threshold, a target building's operational status is set to false.

REQ-017: Tile upkeep scales exponentially beyond 20 tiles (tiles 1–10 linear, 11–20 at ×1.5, 21+ at ×2.5), making large empires structurally fragile.
  *Testable when:* upkeep values for tiles 10, 15, and 25 match the formula — tile 15 at exactly 1.5× per-tile base, tile 25 at exactly 2.5× per-tile base; verifiable by unit test of the upkeep calculation function.

### Labor — Peasants

REQ-020: Peasants are tracked as an aggregate population count per tile with a single collective happiness score.
  *Testable when:* querying all pop records for an inhabited tile returns exactly one record with a Population integer and a collective Happiness float, with no individual name fields present.

REQ-021: Goods tax is a player-set percentage of peasant output, applicable only to goods that tile's population actually produces.
  *Testable when:* (a) setting a wool tax on a grain-only tile returns a validation error; (b) setting a grain tax on a grain-producing tile succeeds; (c) Phase 2 deducts tax only from the qualifying output type.

REQ-022: Corvée labor is a fixed annual obligation in worker-days per tile, used for major construction. The church automatically claims a portion that cannot be redirected.
  *Testable when:* (a) the church's auto-claim is deducted from the corvée pool before player-accessible corvée is calculated; (b) a player action to redirect the church portion is rejected by the server.

REQ-023: Levying corvée during planting or harvest seasons reduces goods tax yield for that cycle.
  *Testable when:* corvée levied during Spring produces a measurably lower goods tax yield that cycle compared to the same levy during Winter, all other variables equal.

REQ-024: Peasant unhappiness escalates: petty crime → significant crime and authority drain → banditry and hidden output → revolt (NPC army, tile control contested).
  *Testable when:* peasant happiness is set to each defined threshold in a test environment and the correct consequence event fires at each level.

### Labor — Specialists

REQ-030: Each specialist is a named individual record with a primary trade at a skill rating, optional secondary skills, a personal inventory, and an individual happiness value.
  *Testable when:* a created specialist record exposes Name, PrimaryTrade, SkillRating, SecondarySkills, Inventory, Gold, Happiness, and CommissionCapacity fields with valid types.

REQ-031: Specialists pay goods tax on finished output, not raw inputs.
  *Testable when:* a blacksmith completes a production cycle and the Phase 2 tax deduction is applied to iron tools produced, not to the iron ore consumed.

REQ-032: Specialists operate private businesses. The lord taxes, commissions, and provides legal and physical framework. The market mediates all transactions.
  *Testable when:* a specialist with no player commission active autonomously purchases inputs, produces output, pays tax, and lists surplus across all applicable phases without a player action each tick.

REQ-033: Each specialist has a commission capacity (typically 20–40% of working time) the lord can direct. Emergency full requisition is available but damages loyalty if used repeatedly.
  *Testable when:* (a) with commission at 30%, commissioned output does not exceed 30% of total production that tick; (b) emergency requisition yields 100% of output and records a loyalty penalty event.

REQ-034: Specialists can depart when loyalty drops: warning → winding down → departure. A rival city with a spy or herald in range may intercept and recruit a departing specialist.
  *Testable when:* loyalty is set below threshold and a departure warning event fires; if uncorrected for N ticks, a departure event fires and the specialist record is removed from the city pop list.

REQ-035: Craft Rating is a continuous 1–100 scale per industry. It climbs with sustained specialist employment and relevant guild buildings. It decays if specialists leave or buildings fall into disrepair.
  *Testable when:* (a) sustained specialist employment for N ticks increases the industry's Craft Rating value; (b) dismissing all specialists causes it to decrease toward its floor value over subsequent ticks.

REQ-036: Specialists exist in three tiers within each trade: Apprentice, Journeyman, Master. Masters are scarce enough that losing one to a rival is a meaningful strategic event.
  *Testable when:* three specialists at different tiers are given identical inputs and Craft Rating conditions — the Master-tier's output has a statistically higher quality value than Journeyman, who is higher than Apprentice. Note: "meaningful strategic event" is a design intent, not a testable condition.

REQ-037: Specialists are attracted by conditions crossing a threshold, at which point a named candidate arrives with a starting skill level and personality traits. The player approves or declines.
  *Testable when:* attraction conditions for a specialist type are met and a candidate record is created on the server; the approve action adds them to the city pop list; the decline action removes the candidate record.

### Labor — Retainers

REQ-040: Retainers are salaried named individuals who wait for orders and do nothing until assigned.
  *Testable when:* a retainer created with no assigned order shows no output, no service, and no state change attributable to their action after N ticks.

REQ-041: Retainer types: Professional Soldier, General, Knight, Spy, Diplomat, Herald.
  *Testable when:* creating a retainer record with an invalid Type value returns a server validation error; all six valid type values are accepted.

REQ-042: Retainer salary is deducted from the city treasury every tick. Missed payments escalate: morale warning → effectiveness penalty and desertion → mass desertion and potential defection with forces.
  *Testable when:* treasury is set to 0 and a morale warning event fires after tick 1; effectiveness penalty and initial desertion fire after tick 2; sustained non-payment triggers mass desertion and general defection events.

REQ-043: Knights and Generals have mandatory luxury consumption as part of their upkeep, not optional.
  *Testable when:* an attempt to set a Knight or General's luxury consumption to zero is rejected by the server; if luxury goods are absent from their inventory in Phase 3, an unhappiness penalty fires.

REQ-044: Equipment degrades over ticks and must be replenished from the stockpile to maintain effectiveness.
  *Testable when:* a retainer soldier's EquipmentCondition field decreases by the defined wear rate per tick; effectiveness decreases below a defined threshold; restocking goods from the stockpile restores EquipmentCondition.

### City Stockpile

REQ-050: The city stockpile is the lord's personal inventory, distinct from the city market. Goods in the stockpile do not participate in market phases automatically.
  *Testable when:* goods placed in the stockpile are absent from market listings until the surplus listing phase executes and quantity exceeds the ceiling.

REQ-051: Stockpile sources: goods tax from peasant tiles (Phase 2), commissioned output, lord purchases (Phase 6), vassal tribute.
  *Testable when:* running a full tick with all four sources active confirms each deposits into the stockpile at the correct phase.

REQ-052: The lord sets per-good buy parameters: minimum threshold, maximum ceiling, maximum price. Unmet orders do not create debt.
  *Testable when:* (a) stockpile falls below minimum threshold and a purchase fires in Phase 6; (b) market price exceeds max-price and the order does not fill; (c) treasury balance does not go negative from an unfilled order.

REQ-053: Goods above the stockpile maximum ceiling are listed on the city market at the end of Phase 3.
  *Testable when:* stockpile quantity exceeds the ceiling and the excess appears in market listings at the end of Phase 3 of the same tick.

REQ-054: During a siege, merchant access is cut off. Stockpile depth at siege onset determines how long the city holds before retainer effectiveness degrades.
  *Testable when:* a siege state is applied and merchant Phase 4/6 participation is disabled; retainer effectiveness decreases at a rate proportional to stockpile depletion over subsequent ticks.

### Guild System

REQ-060: The guild mediates between craftsmen and the lord per trade. The player interacts with the guild as a collective, not individual specialists.
  *Testable when:* a commission rate set with the guild is reflected on all specialists in that trade without individual negotiation actions.

REQ-061: The guild hall accelerates Craft Rating and trains apprentices. Without it, specialists operate individually and are harder to direct and easier to lose one by one.
  *Testable when:* (a) Craft Rating growth rate per tick is higher with a guild hall present than without, all else equal; (b) without a guild hall, individual specialist departure rate is higher under equivalent loyalty conditions.

REQ-062: Guild standing determines available commission capacity and price. Good standing enables self-regulation of apprentice training and quality.
  *Testable when:* guild standing set to poor produces lower available commission capacity and higher commission price than good standing, all other factors equal.

### Education

REQ-070: Literacy is a prerequisite for secondary skills (accounting, alchemy, tactics). Specialists without literacy advance only through guild training, which is slower.
  *Testable when:* (a) a specialist without Literacy attempts to unlock Accounting — the unlock is rejected; (b) a specialist with Literacy successfully unlocks Accounting; (c) Craft Rating growth rate is lower for a non-literate specialist than a literate one in otherwise identical conditions.

REQ-071: Books increase Craft Rating growth rate. A city with a scriptorium and steady book supply trains specialists measurably faster.
  *Testable when:* two identical cities are compared — one with a scriptorium and book supply, one without — and the city with books shows a measurably higher Craft Rating growth rate per tick.

REQ-072: Education buildings: Scriptorium, Guild Hall, Apothecary School, Military Academy, Counting House, University.
  *Testable when:* each building is constructed with its defined requirements met and the expected skill unlock or training multiplier is applied and queryable on affected specialists.

### Merchant System

REQ-080: Merchants are named specialist records that physically travel between cities and participate in the economy tick of whichever city they are currently in.
  *Testable when:* a departing merchant is absent from their home city's pop list and present in the destination city's pop list on arrival tick.

REQ-081: Merchants participate in Phase 4 (list carried goods) and Phase 6 (purchase goods to carry out) only. No Phase 1 production, no Phase 5 stockpile access in foreign cities.
  *Testable when:* a merchant present in a foreign city appears in Phase 4 and Phase 6 processing records but not in Phase 1, 2, or 5 records.

REQ-082: Each merchant maintains a trade ledger of personally observed prices. Ledger entries age. Merchants share ledger entries with guild members in the same city.
  *Testable when:* (a) a merchant visit creates a ledger entry; (b) the entry's age value increments each tick; (c) after N ticks, the entry's influence on routing decisions is lower than a fresh entry for the same good.

REQ-083: Merchant routing follows standing instructions set at departure. No mid-journey recalculation.
  *Testable when:* market conditions change while a merchant is in transit and the merchant's planned itinerary does not change; routing logic is not re-executed until the start of the next journey leg.

REQ-084: Merchant travel range is bounded by home city prestige.
  *Testable when:* routing a merchant to a destination beyond the prestige-derived range returns a validation error; raising prestige increases the maximum allowed range.

REQ-085: Host cities collect a market fee on foreign merchant sales. Home city collects a periodic guild charter fee. Cities controlling strategic chokepoints can assert staple rights.
  *Testable when:* (a) a foreign merchant sale increases the host city treasury by the defined market fee percentage; (b) a merchant returning home triggers the charter fee deduction on the defined schedule; (c) a city with staple rights asserted causes passing merchants to list goods for the defined minimum tick count before continuing.

### Seasons & Time

REQ-090: 1 real minute = 1 game day = 1 server tick (during the activity window).
  *Testable when:* the tick processor fires and the game day counter increments by exactly 1; wall-clock elapsed time between consecutive ticks is 60 ± tolerance seconds.

REQ-091: 1 game year = 480 game days = 8 real hours = 4 seasons of 120 days each.
  *Testable when:* 480 ticks complete and the game year counter increments; season transitions are recorded at ticks 120, 240, 360, and 480.

REQ-092: The harvest fires at a specific predictable Autumn tick. Harvest size depends on arable %, tool quality (in peasant tile inventory), peasant population, summer weather events, and corvée timing.
  *Testable when:* (a) the harvest fires on exactly the defined Autumn tick number; (b) harvest output is higher when tools are in peasant inventory vs. absent; (c) output is lower when corvée was levied during Spring of the same year.

REQ-093: Orders submitted during the frozen window execute at window open in player-set priority order. Failed orders report with a reason.
  *Testable when:* (a) an order submitted during the frozen window appears in the execution queue; (b) on window open it executes in the submitted priority order; (c) a conflicting order returns a failure event with a reason field populated.

REQ-094: Session open report delivered to all players at window open: current stores, authority budget, queued orders, diplomatic messages received, visible map changes.
  *Testable when:* the activity window opens and each connected player receives a report containing all five specified data categories within the grace period before tick 1.

### Military

REQ-100: Projecting military force outside territory requires a General. Generals cost Authority upkeep, not gold.
  *Testable when:* a move order for an army beyond city territory is rejected without a general assigned; succeeds with a general assigned; the upkeep deducted is from the Authority pool, not the gold treasury.

REQ-101: Generals have loyalty ratings and can defect, taking their army with them.
  *Testable when:* a general's loyalty is reduced below the defection threshold via test trigger; a defection event fires and the general's assigned army transfers out of player control.

REQ-102: Each general has a command radius; armies beyond it suffer attrition.
  *Testable when:* units positioned beyond the general's command radius have their effectiveness or health decrease by the defined attrition rate each tick; units within the radius show no attrition.

REQ-103: Sieges take multiple in-game days. Fortification ratings slow progress. Allies can send relief during an ongoing siege.
  *Testable when:* siege progress does not reach completion in a single tick regardless of attacker strength; a fortified city's progress rate is lower than an unfortified one; introducing a relief force pauses or reduces progress on the tick it arrives. Note: "many game-days" is a design intent — the specific minimum duration is a tuning parameter, not a fixed requirement.

REQ-104: No military movement or tile claiming occurs outside the activity window.
  *Testable when:* a military movement action issued during the frozen window is queued but produces no unit position change until window open.

### Diplomacy

REQ-110: All diplomatic communication is a messenger unit that physically travels across the tile map.
  *Testable when:* sending a message creates a messenger entity record with a planned path; no message record exists at the destination until the messenger's position matches the destination tile.

REQ-111: Messengers move a fixed number of tiles per tick. They cannot enter hostile claimed tiles. Path is calculated at send time.
  *Testable when:* (a) a messenger's path routes around hostile tiles; (b) the position advances by exactly the defined tiles-per-tick value each tick.

REQ-112: Messenger range resets and extends each time the path passes through or adjacent to a friendly or neutral city (waypoint).
  *Testable when:* a messenger passing through a waypoint city has their remaining range reset; the total path length supported exceeds what base range alone would allow.

REQ-113: Waypoint cities know a messenger passed through. Messengers can be intercepted via espionage: released, ransomed, or executed.
  *Testable when:* (a) a waypoint city receives a transit notification event; (b) an intercept action succeeds and the capturing player can read message contents; (c) each disposition option produces its specified outcome.

REQ-114: Server-enforced treaty types: Trade League, Defense Pact, Suzerainty. Breaking an in-game treaty costs Reputation, visible to all players.
  *Testable when:* breaking a treaty decreases the breaking city's Reputation field by the defined amount; the change is present in all players' next session open report.

REQ-115: In-game messages can carry verified game state (gold reserves, population census, signed treaties) that out-of-game communication cannot provide.
  *Testable when:* a message sent with attached game state displays values that match the server's authoritative record for that city at send time. Note: the claim that out-of-game communication "cannot provide" this is not testable — it is a design intent for player incentive.

### Vassal System

REQ-120: When a defeated city is vassalized it keeps its city and continues playing. The attacker receives tribute (percentage of trade output) and prestige from dominion.
  *Testable when:* a vassal relationship is created and the vassalized city remains operational; the attacker's treasury receives the defined tribute percentage each tick; the attacker's prestige increases by the defined dominion amount.

REQ-121: Vassals can be liberated by allied forces or buy independence with gold.
  *Testable when:* (a) an allied force reaching a vassalized city makes a liberation action available; (b) the vassal's gold meeting the buyout price allows an independence purchase that removes the vassal relationship.

REQ-122: Over-vassalizing drains Authority rapidly — conquest is a calculated economic decision.
  *Testable when:* Authority upkeep equals N × the per-vassal cost; at the maximum practical vassal count derived from typical authority income, the projected Authority budget is negative.

### Manor System

REQ-130: Each inhabited tile is administered through a manor held by a noble house.
  *Testable when:* claiming an inhabited tile creates a manor record for that tile if one does not exist; uninhabited tiles have no associated manor record.

REQ-131: Nobles take a cut of peasant output before the city tax share, in exchange for local administration and a fixed feudal levy per season.
  *Testable when:* when a peasant tile produces output, the noble cut is deducted before city tax is applied; the feudal levy count is added to the military roster at the start of each season.

REQ-132: Noble loyalty determines how much tax passes through and whether the military obligation is delivered reliably. Low-loyalty nobles may defect.
  *Testable when:* tax pass-through rate at high loyalty vs. low loyalty differs by the formula-defined amount; at minimum loyalty, a defection event becomes available.

REQ-133: An accountant specialist reduces the amount nobles can skim regardless of loyalty.
  *Testable when:* noble skim percentage with an accountant present is lower than without one at equal loyalty levels; the reduction applies regardless of whether loyalty is high or low.

---

## Non-Functional Requirements

REQ-200: All authoritative game state lives on the server and is validated there. The client is treated as untrusted throughout.
  *Testable when:* a manipulated game state payload submitted directly to an action endpoint is rejected; verifiable by code review confirming no economy calculation functions exist in client source.

REQ-201: No secret business logic in the client — economy calculations, cheat detection, and rule enforcement are server-exclusive.
  *Testable when:* the compiled client binary contains no economy calculation functions; verifiable by static analysis or code review of client source.

REQ-202: Client binary hash is verified server-side at authentication.
  *Testable when:* a client with a modified binary hash value attempts to authenticate and the server returns an authentication failure without processing any actions.

REQ-203: All endpoints are rate limited.
  *Testable when:* a client sending requests exceeding the defined rate to any endpoint receives HTTP 429 responses on subsequent requests within the rate window.

REQ-204: Fog of war enforced server-side — full state sync returns only tiles within the player's current visibility range.
  *Testable when:* a full state sync response contains no tile data for tiles outside the player's current visibility range, verified by comparing the response against the server-side visibility calculation.

REQ-205: Action responses return a typed delta (only changed fields) rather than full state. Full state sync corrects drift on reconnect.
  *Testable when:* (a) an action that changes two fields returns a response containing exactly those two fields and no others; (b) a reconnecting client requesting a full sync receives all fields.

REQ-206: WASM is treated as a public API. No game logic should be derivable from client-side decompilation.
  *Non-testable as a discrete test case:* this is a design posture, not a functional behavior. Verifiable by code review and build inspection confirming no secret logic is present in client code.
