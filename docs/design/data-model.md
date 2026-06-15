# Data Model

## Tile

| Field | Type | Notes |
|---|---|---|
| TileId | identifier | |
| Altitude | float | Affects temperature, mineral availability, arable % |
| Temperature | float | Derived from latitude and altitude |
| Precipitation | float | Determines crop viability, river/wetland formation |
| ArablePercentage | float | Derived from altitude, temperature, precipitation; defines farmable portion |
| TerrainType | enum | Forest, Pasture, Meadow, Mineral, Coastal, River, Desert, etc. |
| Resources | list | Resource type + quantity per tile |
| ControllingCity | ref City | Null if unclaimed |
| Peasants | ref PeasantPool | Null if uninhabited |
| Manor | ref Manor | Null if no noble assigned |

Forest tiles have ArablePercentage near zero. Clearing permanently raises ArablePercentage and removes the Timber resource — irreversible.

## PeasantPool

Tracked as aggregate, not individual records.

| Field | Type | Notes |
|---|---|---|
| TileId | ref Tile | One pool per inhabited tile |
| Population | int | Grows over time; shrinks with famine, disease, unrest |
| Happiness | float | Collective score. Drives authority income and cascade consequences |
| Inventory | map Good → Quantity | Includes tools, food buffer, raw materials |
| GoodsTaxRate | float | % of output requisitioned. Must be a good this tile actually produces |
| CorveeDaysOwed | int | Per-tile annual labor obligation |

## Specialist

Named individual record.

| Field | Type | Notes |
|---|---|---|
| SpecialistId | identifier | |
| Name | string | |
| HomeCity | ref City | |
| PrimaryTrade | enum | Blacksmith, Brewer, Weaver, etc. |
| SkillRating | float | 1–100; Apprentice / Journeyman / Master tier thresholds |
| SecondarySkills | list | Literacy, Accounting, Alchemy, Military Tactics — unlocked via education |
| Inventory | map Good → (Quantity, Quality) | Personal inventory |
| Gold | int | Personal gold |
| Happiness | float | Individual value; departure triggers below threshold |
| CommissionCapacity | float | % of working time available for lord direction (typically 20–40%) |
| Loyalty | float | Affects departure risk and rival poaching vulnerability |

## Retainer

Named individual record, salaried directly from city treasury.

| Field | Type | Notes |
|---|---|---|
| RetainerId | identifier | |
| Name | string | |
| Type | enum | Professional Soldier, General, Knight, Spy, Diplomat, Herald |
| Salary | int | Gold deducted from treasury per tick |
| Happiness | float | Individual value |
| CurrentOrder | ref Order | Null when idle |
| Inventory | map Good → (Quantity, Quality) | Personal inventory |
| Loyalty | float | Affected by pay history, victories, rival poaching |
| EquipmentCondition | float | Degrades over ticks; must be replenished from stockpile |

**General additional fields:**

| Field | Type | Notes |
|---|---|---|
| AuthorityUpkeep | int | Per tick while projecting force outside territory |
| CommandRadius | int | Tiles from general; armies beyond suffer attrition |
| ArmyAssigned | list ref Unit | |

## Noble / Manor

| Field | Type | Notes |
|---|---|---|
| NobleId | identifier | |
| Name | string | House name |
| TilesHeld | list ref Tile | |
| Loyalty | float | Determines tax pass-through rate and military obligation reliability |
| FeudalLevyObligation | int | Fixed military units owed per season |

## City

| Field | Type | Notes |
|---|---|---|
| CityId | identifier | |
| OwnerId | ref Player | |
| Prestige | float | Continuous rate output per game-day |
| Authority | float | Current pool; income minus expenditure per tick |
| AuthorityBudget | float | Projected daily balance |
| Treasury | int | Gold |
| CraftRatings | map Trade → float | Per-industry quality score (1–100) |
| Happiness | float | Citywide aggregate |
| CrimePool | float | Accumulates on authority deficit; decays on positive budget |
| Stockpile | ref Stockpile | Lord's personal inventory |
| Market | ref Market | City market |
| Buildings | list ref Building | |
| Specialists | list ref Specialist | |
| Retainers | list ref Retainer | |
| ClaimedTiles | list ref Tile | |

## Stockpile

| Field | Type | Notes |
|---|---|---|
| Goods | map Good → (Quantity, Quality) | Lord's personal inventory |
| BuyOrders | list ref BuyOrder | Per-good: min threshold, max ceiling, max price |

## Market

| Field | Type | Notes |
|---|---|---|
| Listings | list ref Listing | Good, quantity, quality, price, seller |
| PriceHistory | map Good → list float | For price adjustment logic each tick |

Price per good bounded between 20% and 300% of base value. Listings show quality alongside quantity.

## Merchant

Mobile specialist type with a travel schedule.

| Field | Type | Notes |
|---|---|---|
| MerchantId | identifier | |
| HomeCity | ref City | |
| CurrentLocation | ref Tile or City | |
| Itinerary | list ref City | Journey legs |
| TicksRemainingInTransit | int | |
| Inventory | map Good → (Quantity, Quality) | Carried goods |
| TradeLedger | map (City, Good) → LedgerEntry | Personally observed prices; entries age |
| Gold | int | |
| StandingInstructions | ref Instructions | Free trade / Priority good / Fixed route / Seek and return / Embargo |

## Messenger

| Field | Type | Notes |
|---|---|---|
| MessengerId | identifier | |
| Sender | ref City | |
| Recipient | ref City | |
| Contents | encrypted | |
| CurrentTile | ref Tile | |
| PlannedPath | list ref Tile | Calculated at send time |
| Status | enum | InTransit, Captured, Delivered, Lost |
| TravelSpeedPerTick | int | |

## DiplomaticAgreement

| Field | Type | Notes |
|---|---|---|
| AgreementId | identifier | |
| Type | enum | Trade League, Defense Pact, Suzerainty |
| Parties | list ref City | |
| Terms | structured | Server-enforced |
| Status | enum | Active, Broken, Expired |
| ReputationCostOnBreak | float | Visible to all players if broken |

## Goods

Goods carry quality at point of production. Quality propagates up production chains.

| Field | Type | Notes |
|---|---|---|
| GoodType | enum | All finished and intermediate good types |
| Quantity | int | |
| Quality | float | 1–100. Derived from Craft Rating + small variance at production |

See [production-chains](../features/production-chains.md) for full chain definitions.
