# Merchant System

## Merchants as Mobile Specialists

Merchants are a specialist type with a personal inventory and a travel schedule. They are owned by a player but physically travel between cities and participate in the economy tick of whatever city they are currently in.

**Merchant participation in host city tick:**
- Phase 4: lists goods they carried in on the host city market
- Phase 6: purchases goods to carry out, using their personal gold

Merchants do not participate in Phase 1 (no production), Phase 2 (tax and salary handled separately), Phase 3 (personal consumption from own inventory), or Phase 5 (no stockpile access in foreign cities).

## Merchant Record

| Field | Description |
|---|---|
| Home city | |
| Current location | Tile or city |
| Travel itinerary | Journey legs and ticks remaining in transit |
| Personal inventory | Goods being carried |
| Trade ledger | Price observations by city and good |
| Available gold | Purchasing power |
| Standing instructions | Routing behavior |

## Trade Ledger

Each merchant maintains a ledger of personally observed prices and availability. Ledger entries age — older observations drive less aggressive purchasing decisions.

Merchants share ledger entries with other merchants from the same guild when present in the same city. Heralds and diplomats can carry price reports between cities, providing market intelligence without sending a merchant.

## Routing Instructions

Merchants follow standing instructions set at departure. Routing logic runs once per journey leg, not every tick. No mid-journey recalculation.

| Instruction | Behavior |
|---|---|
| Free trade | Use ledger to pick best margin opportunity within range |
| Priority good | Carry this specific good, find best price for it |
| Fixed route | Travel A → B → C → A, buy and sell opportunistically |
| Seek and return | Find a specific good, buy as much as budget allows, come home |
| Embargo | Do not trade with specific city regardless of margins |

**Free trade routing — runs once at departure:**
1. Check ledger entries within travel range
2. Find goods where observed sell price at destination minus observed buy price here exceeds margin threshold
3. Rank by expected profit adjusted for ledger age
4. Pick destination and target good
5. Commit and travel

On arrival the merchant observes actual Phase 4 listings, executes what they can, and updates the ledger for the next leg.

## Merchant Range

Merchant travel range is bounded by home city prestige. Low prestige cities sustain only short-range merchants. High prestige unlocks longer routes. This is the mechanical implementation of prestige-as-trade-range.

## Merchant Recruitment

Merchants are attracted the same way other specialists are — by market conditions, guild hall presence, trade volume, and prestige. The player licenses merchants by providing capital and a charter. The merchant then operates autonomously within their instructions.

## Taxation

**Host city market fee:** When a merchant sells goods in a foreign city during Phase 6, the host city assesses a market fee — a percentage of the sale transferred to the host city treasury. This is the primary taxation mechanism for mobile merchants.

**Home city guild charter fee:** The merchant's home city collects a periodic flat fee for the guild charter giving the merchant the legal right to trade. Paid when the merchant is home. Guaranteed minimum revenue regardless of journey performance.

**Staple rights (city policy):** A city controlling a strategic chokepoint can assert staple rights. Foreign merchants whose route passes through must list goods for sale there for a minimum number of ticks before continuing. The host city gets first access and collects market fees. The merchant is delayed but gains a safe passage guarantee.
