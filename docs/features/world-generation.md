# World Generation

**Status:** Draft

The map is generated procedurally. See ADR-006 for the generation approach and `docs/notes/map-generation-research.md` for full technical research.

## Tile Shape

**Hex tiles with axial (q, r) coordinates.** Every neighbor is equidistant from center — square grid diagonals are ~41% farther, which distorts movement and range. See ADR-005.

## Terrain Parameters Per Tile

| Parameter | Description |
|---|---|
| Altitude | Affects temperature, mineral availability, and arable percentage |
| Temperature | Derived from latitude (cosine curve) + altitude (lapse rate) |
| Precipitation | Derived from latitude bands + altitude; determines crop viability and river/wetland formation |
| Arable Percentage | Derived from altitude, temperature, and precipitation; defines the farmable portion of the tile |

These are generated values from the map generation pipeline — not player-set.

No two cities begin with identical natural specializations. Asymmetric starting terrain makes trade necessary rather than optional.

## Map Generation Pipeline (Option A — Starting Point)

1. Elevation from layered Simplex noise (fractal brownian motion)
2. Temperature from latitude + elevation
3. Rainfall from latitude bands + elevation modulation
4. Hydraulic erosion passes (1–2)
5. River tracing from high-elevation tiles downhill
6. Lakes at local minima receiving flow
7. Biome assignment from (temperature, rainfall)

More realistic options (Voronoi plate tectonics, full atmospheric simulation) can be layered in as needed. See ADR-006.

## Forest Tiles

Forest tiles have zero or near-zero arable percentage and produce timber. Clearing a forest tile **permanently** raises its arable percentage and removes the timber resource. This is an irreversible decision with long-term strategic consequences.

A Forester specialist can manage sustainable partial harvest — preventing full deforestation while still producing timber over time.
