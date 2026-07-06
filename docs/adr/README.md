# Architecture Decision Records

One file per significant architectural decision. Each records the context at the time, the decision made, the alternatives rejected, and the consequences.

## Append-only rules

ADR files are a permanent record. They are **append-only**.

| Situation | What to do |
|---|---|
| Clarification or consequence discovered | Append an `## Amendment — YYYY-MM-DD` section at the bottom. Leave all original content intact. |
| Decision partially revised | Append an `## Amendment — YYYY-MM-DD` section explaining what changed and why. |
| Decision fully superseded | Create a new ADR (`007-`, `008-`, etc.). Append one line to the old file updating its Status to `Superseded by ADR-XXX`. |
| New decision | Create a new file with the next sequential number. |

The **only** permitted in-place edit to an existing ADR is updating the `Status:` line.

## Index

| ADR | Title | Status |
|---|---|---|
| [001](001-server-authoritative-economy.md) | Server-Authoritative Economy | Decided |
| [002](002-client-technology.md) | Client Technology | Decided — Godot 4 |
| [003](003-activity-window.md) | Frozen Activity Window Model | Decided |
| [004](004-delta-api-responses.md) | Typed Delta API Responses | Decided |
| [005](005-hex-grid.md) | Hex Grid with Axial Coordinates | Decided |
| [006](006-map-generation-approach.md) | Map Generation Approach | Decided — Option A |
| [007](007-client-technology.md) | Client Technology: Blazor WebAssembly | Decided |