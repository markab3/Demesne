# City Stockpile

**Status:** Draft

The city stockpile is the lord's personal inventory, distinct from the city market. Goods in the stockpile do not participate in market phases automatically.

## Sources

- Goods tax collected from peasant tiles — deposited directly in Phase 2
- Commissioned output the lord has paid for — delivered directly
- Lord purchase orders executed during Phase 6
- Tribute from vassals

## Stockpile Flow Per Tick

```
+ Goods tax received (Phase 2)
+ Lord purchases from market (Phase 6)
- Retainer requisitions (Phase 5)
- Surplus listed on market if above maximum ceiling (end of Phase 3)
```

## Lord Buy Orders

The lord sets per-good buy parameters:
- **Minimum threshold** — buy if stockpile falls below this
- **Maximum ceiling** — buy up to this quantity
- **Maximum price willing to pay**

If a good is unavailable or above the price limit, the order does not fill that tick. No debt is created. This is the pressure that drives the lord to attract merchants or commission local production.

## Surplus Listing

Stockpile goods above the maximum ceiling are listed on the city market at the end of Phase 3. The lord actively offloads excess rather than hoarding indefinitely.

## Siege Relevance

During a siege, market activity collapses and merchant access is cut off. Stockpile depth at siege onset determines how long the city holds before retainer effectiveness degrades. Stocking the stockpile before a siege is a deliberate defensive preparation.
