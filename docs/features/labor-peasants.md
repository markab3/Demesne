# Labor — Peasants

**Status:** Draft

Peasants are never tracked individually. They are a population count attached to a tile with a single collective happiness score.

## Peasant Pool

- Scales with controlled tiles, especially inhabited and arable ones
- Grows slowly over time as population expands onto productive land
- Shrinks with famine, disease, military losses, or sustained unrest

## What Peasants Produce

- Grain and other crops
- Raw wool, raw hide, raw timber via tile resource
- Corvée labor days (separate from goods tax)
- Basic gathered materials — clay, sand, surface minerals

Peasants cannot produce anything requiring skilled technique. A peasant harvests wheat; a miller turns it into flour. This boundary is strict.

## Goods Tax

A player-set percentage of peasant output, applicable only to goods that tile's population actually produces. You cannot tax grain farmers in wool.

| Tax Rate | Effect |
|---|---|
| 10–20% | Peasants retain surplus; happiness neutral |
| 30–40% | Subsistence farming with little buffer; happiness begins declining |
| 50%+ | Peasants begin to hunger; happiness drops sharply; population growth stalls or reverses |

As peasant population grows, yield grows proportionally.

## Corvée Labor

A fixed annual labor obligation in worker-days per tile, used for major construction: roads, walls, large civic buildings.

- The obligation is fixed — more tiles and peasants are needed for more labor, not a higher rate
- The church automatically claims a portion of the corvée pool for ecclesiastical construction — this cannot be redirected
- Levied peasants are pulled from farming during the levy period — timing badly (during planting or harvest) cuts goods tax yield for that cycle
- Sustained or repeated levies exhaust peasants, generate unrest, and reduce next season's output
- Optimal construction window is winter, when agricultural impact is lowest

## Unhappiness Cascade

| Level | Consequence |
|---|---|
| Mild | Petty crime pool begins filling |
| Moderate | Crime significant, authority drain begins |
| High | Banditry spawns on tile; peasants hide output, reducing effective tax yield |
| Critical | Revolt — NPC army spawns on tile, tile control contested |
