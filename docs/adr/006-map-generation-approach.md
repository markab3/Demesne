# ADR-006: Map Generation Approach

Status: Decided — Option A as starting point

## Context

Three procedural map generation approaches were evaluated for producing realistic, asymmetric maps where terrain creates genuine economic specialization pressure. See `docs/notes/map-generation-research.md` for full technical detail.

## Options

| | Option A | Option B | Option C |
|---|---|---|---|
| Approach | Simplex Noise + climate bands | Voronoi plate tectonics | Full atmospheric simulation |
| Complexity | Low | Medium | High |
| Realism | Good | Great | Excellent |
| Rivers | Steepest-descent | Steepest-descent | Flow accumulation |
| Climate | Latitude bands | + Ocean distance | Full wind simulation |
| Erosion | Simple sediment | Simple sediment | Particle hydraulic + thermal |

## Decision

**Start with Option A (Simplex Noise + climate bands). Layer in Option B and C components as needed.**

Option A produces maps that satisfy the core design requirement — asymmetric terrain that makes trade feel necessary — without the implementation complexity of full simulation. Options B and C can be layered in incrementally if Option A maps feel too regular or lack geologically plausible features (believable mountain chains, ocean basins).

## Option A Pipeline

1. Generate elevation with layered Simplex noise (fractal brownian motion / fBm)
2. Assign latitude from Y position
3. Derive temperature: latitude (cosine curve) + elevation lapse rate (−6.5°C per 1000m)
4. Derive rainfall: latitude bands modulated by elevation
5. Run 1–2 hydraulic erosion passes (sediment fraction moves downhill)
6. Trace rivers: flow downhill from high-elevation tiles, accumulate flow counter
7. Place lakes at local minima receiving flow with no lower neighbor
8. Assign biomes from (temperature, rainfall) lookup

## Consequences

- Tile terrain parameters (altitude, temperature, precipitation, arable %) are derived values from this pipeline
- Map is deterministic given a seed — reproducible for testing and instance replay
- Option B plate tectonic step can be inserted before step 1 to replace or augment noise-based elevation
- Option C atmospheric simulation can replace the latitude-band rainfall in step 4
