# Economy Tick

Every game-day tick processes all pops in a city through six phases in order. One tick fires per real minute during the activity window.

## Phase 1 — Production

All pops attempt to produce their output. Input checking happens at phase start — a pop either has required inputs in inventory or produces nothing that tick. Partial inputs produce nothing.

**Peasant tile yield formula inputs:**
- Base tile arable percentage and terrain values
- Season modifier (zero in winter, peak at harvest tick)
- Tools present in peasant tile inventory — specific yield multiplier per tool quality
- Peasant population count on tile
- Noble or steward quality modifier

Tools must be physically in the peasant tile's inventory to apply the yield multiplier. A blacksmith does not boost output by existing — his tools do, once they have been purchased by peasant tiles through the market.

**Specialist production:** Output quantity is deterministic based on inputs present. Output quality is a function of Craft Rating plus small variance.

## Phase 2 — Taxes & Salary

Goods tax is deducted from peasant and specialist inventories and deposited into the city stockpile.

Retainer salaries are paid from the city treasury into retainer personal inventories. Salary is paid before any tax assessment on salary income.

If the city treasury cannot cover salary obligations, missed payment penalties begin.

## Phase 3 — Consumption

All pops consume personal needs from their own inventories simultaneously. Food is eaten. Clothing degrades at a slow rate. Unmet needs generate unhappiness penalties immediately.

Retainers consume from their personal inventory at this stage — using salary received in Phase 2. No stockpile access yet.

Stockpile goods above the lord's maximum ceiling are listed on the city market at the end of this phase.

## Phase 4 — List for Sale

All pops list surplus inventory on the city market. Surplus is defined as inventory above their personal comfort threshold per good.

**Price adjustment based on last tick:**
- Goods returned unsold: price drops
- Goods completely sold out: price rises slightly
- Price bounded between 20% and 300% of base value

Listings show quality alongside quantity ("50 iron tools, quality 73").

Retainers do not list goods. They hold surplus personally.

## Phase 5 — Retainer Requisition

Retainers check their inventory against their needs profile. Shortfalls not met in Phase 3 are requisitioned directly from the city stockpile (if available).

This happens before the market opens. Retainers have first access to stockpile goods ahead of the open market.

If the stockpile cannot cover a retainer's requisition, the shortfall carries forward into Phase 6 where the retainer competes in the open market using their salary.

## Phase 6 — Purchases

All pops including the lord and retainers purchase from the city market in a **wealth-ordered round-robin**. The lord slots first by virtue of having the largest treasury.

**Purchase order within a single pop's turn:**
1. Subsistence food
2. Required production inputs (specialists buying raw materials for next Phase 1)
3. Comfort goods — beer, candles, soap
4. Luxury goods — wine, fine clothing, spiced food
5. Capital goods — peasant tiles buying tools, specialists buying better equipment

**Quality preference by buyer:**

| Buyer | Quality Behavior |
|---|---|
| Peasant tile | Buys cheapest available regardless of quality |
| Specialist (inputs) | Buys adequate quality — high quality inputs improve output |
| Specialist (personal) | Moderate quality preference |
| Retainer soldier | Prefers high quality weapons and armor |
| Knight / General | Strong quality preference across all categories |

The round-robin continues until a full cycle completes with no purchases. Remaining goods are returned to sellers and noted as unsold for Phase 4 price adjustment next tick.

Lord purchase orders for stockpile restocking also execute here, within the lord's first-position turn.

## Purchase Priority Summary

| Priority | Who | Mechanism |
|---|---|---|
| 1st | Lord | First position in round-robin by treasury wealth |
| 2nd | Retainers (remaining needs) | Phase 5 stockpile requisition covers most; salary covers remainder in market |
| 3rd | Wealthy pops | Round-robin by wealth |
| 4th | Poor pops | Round-robin, last turns |

Retainer necessities are covered before the market opens via Phase 5 stockpile requisition. The lord buying out the market in Phase 6 therefore affects civilian population primarily, not retainer survival — unless the stockpile was already empty going into Phase 5.

## Goods Physically Matter

The core principle underlying the entire economy: goods must be in a pop's inventory to have effect.

- Iron tools in a peasant tile's inventory → yield multiplier applies next Phase 1
- Iron tools sitting in the lord's stockpile → no effect on farming
- Food in a retainer's inventory → consumed in Phase 3, no unhappiness
- Food in the market but not purchased → retainer goes hungry, unhappiness applies

A city specializing in a single export and importing everything else works correctly under this model. Merchants carry export goods out in Phase 6; gold returns to specialists; specialists buy imported food and tools from arriving merchants in Phase 6; peasant tiles purchase tools; yield increases next Phase 1. The comparative advantage loop closes without scripting.
