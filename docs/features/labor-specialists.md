# Labor — Specialists

**Status:** Draft

## Record Structure

Each specialist is a named individual with:
- A primary trade at a skill rating
- Optional secondary skills unlocked through education
- A personal inventory of goods and money
- An individual happiness value

## Autonomous Economy

Specialists operate private businesses. The lord does not employ them directly — the lord taxes, commissions, and provides the legal and physical framework that makes their business viable.

Each specialist participates in all six tick phases as a normal pop:
- Phase 1: produces finished goods if inputs are in inventory
- Phase 2: pays goods tax on **finished output** (not raw inputs) into city stockpile
- Phase 3: consumes personal needs from own inventory
- Phase 4: lists surplus finished goods and unsold inputs on the market
- Phase 5: no stockpile access
- Phase 6: purchases inputs for next tick's production, then personal needs, then luxuries

The blacksmith buys iron from the market, produces tools, pays tools tax, sells surplus tools on the market, and uses earnings to buy food, clothing, and eventually luxuries. The market mediates everything — the lord does not receive tools directly from the blacksmith.

## Commission Mechanic

Each specialist has a commission capacity — typically 20–40% of their working time — that the lord can direct toward specific outputs. The remainder runs autonomously.

- Commission requires payment above the specialist's normal margin
- Commission competes with private demand — a booming private order book reduces available commission capacity
- **Emergency requisition** commandeers full output at a flat rate. Legitimate in a crisis; repeated use damages loyalty and may cause departure

## Departure Mechanic

1. Happiness drops below threshold — visible warning on their record
2. If unaddressed — they begin winding down private operations
3. Final departure — they leave, Craft Rating contribution lost, skill goes with them
4. A rival city with a spy or herald in range may intercept the departing specialist and recruit them

Specialists do not revolt. Their consequence is purely economic departure.

## Loyalty Factors

| Factor | Effect |
|---|---|
| Commission fees paid fairly | Positive |
| Private trade healthy | Positive |
| City prestige rising | Positive |
| Guild hall maintained | Positive |
| Repeated emergency requisition | Negative |
| Tax rate raised significantly | Negative |
| City under siege or crime high | Negative |
| Rival city offering better conditions | Negative pull |
| Primary input unavailable | Negative |

## Attraction Mechanic

Specialists arrive when conditions are right. The city generates attraction for each specialist type based on:
- Relevant raw materials available locally or in the market
- Guild hall or relevant workshop building present
- Market activity and trade volume
- Prestige level
- Existing specialists in related trades (a tanner attracts a cobbler)
- Food surplus and housing availability
- Tax rate on goods

When attraction for a specialist type crosses a threshold, a named candidate arrives with a starting skill level and personality traits. The player approves or declines.

## Specialist Registry

### Production Specialists

| Specialist | Primary Output | Key Inputs | Notes |
|---|---|---|---|
| Miller | Flour, meal | Grain, millstone | Prerequisite for baker |
| Baker | Bread, hardtack | Flour, fuel | Army supply chain dependent |
| Butcher | Salted meat, tallow, hide | Livestock, salt | Byproduct chain feeds leatherworker |
| Fisherman | Fresh/salted fish | Coastal/river tile, salt | Tile-dependent |
| Brewer | Beer, ale | Barley, hops, water | Happiness specialist |
| Vintner | Wine | Grapes, barrels | Climate-restricted, high prestige |
| Distiller | Spirits | Grain, copper still | Requires brewer as prerequisite |
| Blacksmith | Iron goods, tools, weapons | Iron, coal, fuel | Core military and agricultural chain |
| Weaponsmith | Swords, spears, arrowheads | Iron/steel, leather | Military-focused blacksmith branch |
| Armourer | Plate, chainmail, helms | Iron/steel, leather | Requires master-level metalwork |
| Coppersmith | Copper goods, stills, vessels | Copper | Enables distillery and alchemist chains |
| Goldsmith | Jewelry, coinage, regalia | Gold, silver, gems | High prestige output |
| Silversmith | Tableware, church goods | Silver | Steady church and noble market |
| Mason | Cut stone, brick structures | Limestone, marble, clay | Required for all stone construction |
| Carpenter | Planks, furniture, barrels | Timber | Barrels are a critical intermediate |
| Wheelwright | Wheels, carts, wagons | Timber, iron fittings | Trade efficiency, reduces caravan upkeep |
| Shipwright | Ships, river barges | Planks, rope, pitch, sails | Coastal/river tile requirement |
| Ropemaker | Rope, cordage | Flax, hemp | Shipbuilding and construction prerequisite |
| Weaver | Wool cloth, linen cloth | Yarn, thread | Core textile chain |
| Dyer | Dyed cloth | Cloth, dye crops | Requires dye crop supply |
| Tailor | Clothing, fine garments | Cloth, thread | End of textile chain |
| Furrier | Fur garments, trim | Animal pelts | Luxury clothing variant |
| Tanner | Leather | Raw hide, tannin | Slow process — specialist speeds significantly |
| Cobbler | Boots, shoes, harness | Leather | Army upkeep reduction |
| Bowyer | Bows, crossbows | Timber, sinew, horn | Ranged military unit prerequisite |
| Fletcher | Arrows, bolts | Timber, feathers, iron | Consumed in combat — ongoing demand |
| Potter | Ceramics, vessels, pipe | Clay, fuel | Storage capacity bonus |
| Glassblower | Glass, windows, vessels | Sand/silica, fuel | Prestige buildings require glass |
| Chandler | Tallow and beeswax candles | Tallow, beeswax | Happiness and mine output bonus |
| Soapmaker | Soap | Tallow/olive oil, ash | Happiness and disease resistance |
| Papermaker | Paper | Linen rags, water | Enables faster book production |

### Knowledge & Administrative Specialists

| Specialist | Function | Requirements | Notes |
|---|---|---|---|
| Scribe | Produces books and documents | Literacy, parchment or paper, ink | Craft Rating determines book quality |
| Scholar | Accelerates specialist skill growth citywide | Books, university building | Passive multiplier — high value |
| Physician | Reduces population loss during disease, improves happiness | Herbs, apothecary training | Event-critical during plague |
| Apothecary | Produces tinctures, remedies, soap | Herbs, physician prerequisite | Consumer good and military supply |
| Alchemist | Produces pigments, experimental goods, gunpowder precursors | Books, sulfur, lead, scholar adjacency | Unlocks late-chain exotic goods |
| Cartographer | Extends scouting range, reveals terrain | Books, literacy | Reveals mineral deposits |
| Accountant | Trade efficiency bonus, reduces noble skimming | Counting house, literacy, books | Directly counters manor loyalty drain |
| Lawyer / Notary | Enables binding contracts, reduces reputation loss | Books, literacy | Diplomatic specialist |
| Herald | Messenger speed bonus, diplomatic range extension | Literacy, prestige threshold | Reduces messenger travel time |
| Spy | Intercepts messengers, scouts enemy tiles, poaches specialists | Separate skill tree | High upkeep, deniable |

### Agricultural & Rural Specialists (tile level)

| Specialist | Function | Tile Requirement | Notes |
|---|---|---|---|
| Steward | Manages a manor — reduces noble loyalty drain, improves tax yield | Inhabited tile with manor | Lord assigns one per manor |
| Reeve | Organises corvée efficiently — reduces harvest season penalty | Inhabited tile | Peasant-facing administrator |
| Forester | Manages sustainable timber — prevents full deforestation | Forest tile | Allows partial harvest without permanent clearance |
| Charcoal Burner | Produces charcoal from managed forest | Forest tile | Alternative fuel chain |
| Shepherd | Improves wool and livestock yield | Pasture tile | Feeds textile chain |
| Beekeeper | Produces beeswax and honey | Meadow tile | Candle and luxury food chain |
| Herbalist | Harvests medicinal herbs | Forest or meadow tile | Feeds apothecary chain |
| Miner | Improves mineral extraction rate and quality | Mineral tile | Craft Rating improves ore yield |
| Prospector | Reveals hidden mineral deposits on adjacent tiles | Any tile | One-time survey action, consumed on use |
