# Demesne: Trade & Tribute

Browser-based multiplayer medieval fantasy city-builder. Players grow cities on a shared open-world map, specializing economically and competing through trade, diplomacy, and military force.

## Tech Stack

| Layer | Technology |
|---|---|
| Client | Godot 4 (C#), exported to Web |
| Backend | ASP.NET Core WebAPI |
| Real-time | SignalR (server-pushed tick deltas) |
| Serialization | System.Text.Json / JsonNode |

## Key Decisions

- **Server-authoritative.** All game state lives on the server. The client is a display and input layer — treat it as untrusted throughout. No economy logic, validation, or rule enforcement in the client. (ADR-001)
- **Godot 4 client architecture.** Two autoloaded singletons: `StateManager` (holds display copy of server state, exposes `MergePartial`, emits domain-scoped signals) and `ActionDispatcher` (wraps all HTTP POST calls). (ADR-002)
- **Hex tiles, axial (q, r) coordinates.** Equidistant neighbors; cube coordinate math for distance/range. (ADR-005)
- **Map generation: Simplex noise + climate bands as starting point**, with Voronoi tectonics and full atmospheric simulation available to layer in. (ADR-006)
- **Activity window.** The game runs 8 hours/day. Outside the window it is fully frozen — no ticks, no military movement, no territory loss. All players in an instance share the same window.
- **Economy tick.** Server fires once per real minute during the window. Six phases in order: Production → Taxes & Salary → Consumption → List for Sale → Retainer Requisition → Purchases.
- **Three pop tiers.** Peasants (aggregate pool per tile), Specialists (named individual records, autonomous businesses), Retainers (named, salaried, wait for orders).
- **Delta API responses.** Action responses return only changed fields (sparse delta), not full state. Full sync on login/reconnect. (ADR-004)

## ADR Rules

ADR files in `docs/adr/` are **append-only**. Never delete or edit existing body content.

- **To amend:** append a `## Amendment — YYYY-MM-DD` section at the bottom of the existing file.
- **To supersede:** create a new ADR with the next number; update the old file's `Status:` line to `Superseded by ADR-XXX`. The `Status:` line is the only in-place edit permitted.
- **New decisions:** create a new file (`007-`, `008-`, etc.) and add it to the index in `docs/adr/README.md`.

## Documentation Map

```
docs/
├── adr/                     # Architecture Decision Records — append-only, see README
│   ├── README.md            # Index and append-only rules
│   ├── 001-server-authoritative-economy.md
│   ├── 002-client-technology.md
│   ├── 003-activity-window.md
│   ├── 004-delta-api-responses.md
│   ├── 005-hex-grid.md
│   └── 006-map-generation-approach.md
├── spec/
│   ├── overview.md          # Vision, goals, instance lifecycle
│   └── requirements.md      # Numbered requirements (REQ-xxx) with testability criteria
├── design/
│   ├── architecture.md      # Stack, tick processor, security model, API transport split
│   ├── data-model.md        # Entity definitions and field schemas
│   ├── api.md               # Action envelope, endpoint categories, SignalR events
│   └── frontend.md          # Godot client architecture, StateManager, ActionDispatcher
├── features/                # Behavioral specs — one file per game system
│   ├── world-generation.md
│   ├── resources.md
│   ├── production-chains.md
│   ├── quality-system.md
│   ├── economy-core.md      # Prestige, Authority, Happiness, Crime
│   ├── economy-tick.md      # Six-phase tick in detail
│   ├── labor-peasants.md
│   ├── labor-specialists.md
│   ├── labor-retainers.md
│   ├── guild-system.md
│   ├── city-stockpile.md
│   ├── merchant-system.md
│   ├── education.md
│   ├── seasons-time.md
│   ├── military.md
│   ├── diplomacy.md
│   ├── vassal-conflict.md
│   └── manor-system.md
└── notes/                   # Background research and explored approaches — not binding
    ├── architecture-approaches.md   # Godot + ASP.NET implementation patterns
    ├── map-generation-research.md   # Three gen options, Simplex noise, hex math
    └── medieval-peasant-history.md  # Historical research with design implications
```

## Where to Look

| Task | Start here |
|---|---|
| Understanding a game mechanic | `docs/features/<mechanic>.md` |
| Understanding the tech shape | `docs/design/architecture.md` |
| Understanding an entity's fields | `docs/design/data-model.md` |
| Understanding why a decision was made | `docs/adr/` |
| Checking what is and isn't required | `docs/spec/requirements.md` |
| Understanding a Godot implementation pattern | `docs/notes/architecture-approaches.md` |
| Historical or research context | `docs/notes/` |
