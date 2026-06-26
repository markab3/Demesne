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

## Authority Hierarchy

When sources conflict, this is the order of authority:

1. `docs/spec/requirements.md` — the contract; defines what the system must do
2. `docs/features/<name>.md` — defines how each feature behaves
3. `docs/design/` — defines how it is built
4. `docs/notes/` — background context only; never authoritative

## Workflow

At the start of any session, check `docs/spec/status.md` to confirm the current milestone and what work is in scope.

Before implementing any feature, read the relevant `docs/features/` file and the REQ entries it satisfies in `docs/spec/requirements.md`.

Before changing any existing feature, update the `docs/features/` file first. The spec is always ahead of or equal to the code.

When completing a Foundation item or Milestone task, mark it done in `docs/spec/status.md`.

When making a significant architectural decision, create a new ADR in `docs/adr/`. See ADR Rules above.

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
│   ├── requirements.md      # Numbered requirements (REQ-xxx) with testability criteria
│   └── status.md            # Foundation checklist, Milestones, REQ implementation status
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
| Current milestone and work in scope | `docs/spec/status.md` |
| Understanding a game mechanic | `docs/features/<mechanic>.md` |
| Understanding the tech shape | `docs/design/architecture.md` |
| Understanding an entity's fields | `docs/design/data-model.md` |
| Understanding why a decision was made | `docs/adr/` |
| Checking what is and isn't required | `docs/spec/requirements.md` |
| Understanding a Godot implementation pattern | `docs/notes/architecture-approaches.md` |
| Historical or research context | `docs/notes/` |

# General Code Guideline Instructions

- Always verify information before presenting it. Do not make assumptions or speculate without clear evidence.
- Make changes file by file and allow for review of mistakes.
- Never use apologies or give feedback about understanding in comments or documentation.
- Don't suggest whitespace changes or summarize changes made.
- Only implement changes explicitly requested; do not invent changes.
- Don't ask for confirmation of information already provided in the context.
- Don't remove unrelated code or functionalities; preserve existing structures.
- Provide all edits in a single chunk per file, not in multiple steps.
- Don't ask the user to verify implementations visible in the provided context.
- Don't suggest updates or changes to files when there are no actual modifications needed.
- Always provide links to real files, not context-generated files.
- Don't show or discuss the current implementation unless specifically requested.
- Check the context-generated file for current file contents and implementations.
- Prefer descriptive, explicit variable names for readability.
- Adhere to the existing coding style in the project.
- Prioritize code performance and security in suggestions.
- Suggest or include unit tests for new or modified code.
- Implement robust error handling and logging where necessary.
- Encourage modular design for maintainability and reusability.
- Ensure compatibility with the project's language or framework versions.
- Replace hardcoded values with named constants.
- Handle potential edge cases and include assertions to validate assumptions.

## Naming Conventions
- Use underscores for private fields (e.g., `_privateField`)
- Use lowercase camelCase names without underscores for locally scoped variables (e.g., `localVariable`)
- Use PascalCase without underscores for public items (e.g., `PublicProperty`, `PublicMethod`)

## .NET 9 and Modern C# Guidelines
- Project uses nullable reference types (`<Nullable>enable</Nullable>`)
- Use nullable annotations (?, !, ??) appropriately
- Leverage C# 13 features where beneficial
- Follow .NET 9 performance and security best practices
- Prefer using statements instead of fully specifying namespaces.