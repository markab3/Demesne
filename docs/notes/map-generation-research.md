# Map Generation Research

Source: design chat on procedural map generation for a tile-based city builder.

Recommendation from this session: **Start with Option A, layer in Option B/C components as needed.**

See ADR-006 for the decision record.

---

## Tile Properties Per Cell

Each tile carries:
- Position (latitude/longitude equivalent)
- Elevation
- Temperature
- Rainfall

Climate rules: poles = cold/dry, equator = hot/wet. Rivers and lakes emerge from elevation. Erosion passes smooth terrain.

---

## Option A — Simplex Noise + Climate Bands (Easiest)

1. Generate elevation with layered Simplex noise (fractal brownian motion / fBm)
2. Assign latitude from Y position
3. Derive temperature from latitude (cosine curve) + elevation (lapse rate: −6.5°C per 1000m)
4. Derive rainfall from latitude bands (wet tropics ~0°, dry subtropics ~30°, wet temperate ~50°, dry polar ~70°+) modulated by elevation
5. Run 1–2 hydraulic erosion passes (move sediment fraction downhill per tile)
6. Trace rivers: flow downhill from high-elevation tiles, accumulate a flow counter
7. Place lakes at local minima that receive flow but have no lower neighbor
8. Assign biomes from (temperature, rainfall) via Whittaker diagram

**Best for:** Prototyping. Pure frontend. Easy to tune.

---

## Option B — Voronoi Plate Tectonics (Medium)

1. Scatter N points, build Voronoi diagram → tectonic plates
2. Assign each plate a type (oceanic = low, continental = high) and drift vector
3. Detect boundaries: convergent → mountains, divergent → rifts/trenches, transform → no change
4. Layer Simplex noise on top for local texture
5. Run climate simulation (Option A steps 2–4) + ocean distance factor for rainfall
6. Run erosion (2–3 passes)
7. Trace rivers and lakes (same as Option A)
8. Assign biomes

**Best for:** Geologically grounded maps with believable mountain chains and ocean basins.

---

## Option C — Full Simulation Pipeline (Most Realistic)

1. Elevation via plate tectonics (Option B steps 1–4)
2. Thermal erosion: collapse tiles steeper than angle-of-repose threshold
3. Particle-based hydraulic erosion: virtual raindrops flow downhill, pick up and deposit sediment
4. Atmospheric circulation: Hadley/Ferrel/Polar cells by latitude, prevailing winds, orographic precipitation
5. Temperature from latitude + elevation + ocean proximity
6. Flow accumulation grid for rivers (each tile's flow = its rainfall + sum of uphill neighbors' flow)
7. Lakes via flood-fill from closed basin minima
8. Biomes via Köppen classification
9. Optional: soil fertility from parent rock + erosion + climate

**Best for:** Deep simulation (Dwarf Fortress / Civilization-style) with internally consistent geography.

---

## Comparison

| | Option A | Option B | Option C |
|---|---|---|---|
| Complexity | Low | Medium | High |
| Realism | Good | Great | Excellent |
| Rivers | Steepest-descent | Steepest-descent | Flow accumulation |
| Climate | Latitude bands | + Ocean distance | Full wind simulation |
| Erosion | Simple sediment | Simple sediment | Particle hydraulic + thermal |

---

## Simplex Noise

A gradient noise algorithm (Ken Perlin, 2001). Maps a continuous (x, y) input to a smooth, deterministic value in [−1, 1].

**How it works:**
1. Divide space into triangular simplices (2D)
2. Assign a random gradient vector to each vertex from a fixed permutation table
3. Determine which simplex the sample point falls in
4. Compute each vertex's contribution via dot product + distance falloff
5. Sum contributions → output value

**Simplex vs. Perlin:**

| | Perlin | Simplex |
|---|---|---|
| Grid shape | Squares | Triangles |
| Neighbors sampled | 2ⁿ | n+1 |
| Directional artifacts | More visible | Fewer |

**Fractal Brownian Motion (fBm):** Layer multiple octaves, each doubling frequency and halving amplitude:
```
elevation = 1.0   * noise(x*1, y*1)
          + 0.5   * noise(x*2, y*2)
          + 0.25  * noise(x*4, y*4)
          + 0.125 * noise(x*8, y*8)
```
Produces large-scale shapes with fine detail on top.

**Hex grids don't change noise selection.** Noise operates in continuous 2D space. Convert hex center coordinates to world (x, y), pass to noise.

---

## Hex Grid Coordinate Systems

See ADR-005 for the decision to use hex tiles.

### Cube / Axial Coordinates (q, r, s)

Three axes spaced 120° apart. Constraint: `q + r + s = 0`. Since s is always derivable, only (q, r) need to be stored.

**Six movement directions:**

| Direction | Δq | Δr | Δs |
|---|---|---|---|
| Right | +1 | −1 | 0 |
| Left | −1 | +1 | 0 |
| Upper-right | +1 | 0 | −1 |
| Lower-left | −1 | 0 | +1 |
| Upper-left | 0 | +1 | −1 |
| Lower-right | 0 | −1 | +1 |

**Useful operations:**
```
distance = max(|q1-q2|, |r1-r2|, |s1-s2|)
neighbors = all six (Δq, Δr, Δs) combinations above
```

**Convert to world (x, y) for noise sampling:**

Flat-topped hexes (size R):
```
x = R * (3/2 * q)
y = R * (√3/2 * q + √3 * r)
```

Pointy-topped hexes:
```
x = R * (√3 * q + √3/2 * r)
y = R * (3/2 * r)
```

**Why hex over square:** Every neighbor is equidistant from center. Square grids have diagonal neighbors ~41% farther away, which distorts movement and range calculations.

**Reference:** redblobgames.com/grids/hexagons
