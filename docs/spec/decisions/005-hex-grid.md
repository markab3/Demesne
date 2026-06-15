# ADR-005: Hex Grid with Axial Coordinates

Status: Decided

## Context

The map needs a tile shape that supports movement, range calculations, and adjacency checks. Two primary options: square grid and hex grid.

Square grids have a geometry problem: diagonal neighbors are ~41% farther from center than orthogonal neighbors. This distorts movement costs, range calculations, and influence radius in ways that require compensating workarounds.

## Decision

**Hex tiles with cube/axial (q, r) coordinate system.**

Every hex neighbor is equidistant from center. No diagonal distortion. Movement, range, and adjacency are geometrically consistent.

## Coordinate System

Cube coordinates: three axes spaced 120° apart. Constraint: `q + r + s = 0`. Only (q, r) need to be stored — s is always derivable.

**Six movement directions:**

| Direction | Δq | Δr | Δs |
|---|---|---|---|
| Right | +1 | −1 | 0 |
| Left | −1 | +1 | 0 |
| Upper-right | +1 | 0 | −1 |
| Lower-left | −1 | 0 | +1 |
| Upper-left | 0 | +1 | −1 |
| Lower-right | 0 | −1 | +1 |

**Distance:** `max(|q1−q2|, |r1−r2|, |s1−s2|)`

**Convert to world (x, y) for noise sampling** — flat-topped hexes (size R):
```
x = R * (3/2 * q)
y = R * (√3/2 * q + √3 * r)
```

## Consequences

- Tile addressing uses (q, r) pairs, not (x, y) pixel coordinates
- Noise functions for map generation sample at the world (x, y) positions of hex centers
- All range, movement, and adjacency math uses cube coordinate operations
- Reference implementation: redblobgames.com/grids/hexagons
