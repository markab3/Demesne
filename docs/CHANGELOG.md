# Changelog

## ADRs 005–006 added; name set; notes directory created (2026-06-14)

### Game name
Set to **Demesne: Trade & Tribute** in CLAUDE.md and overview.md.

### docs/notes/ directory created
Background research and design exploration preserved here — not spec, not decisions, but context that informed or may inform the design.

- `architecture-approaches.md` — Godot 4 + ASP.NET Core implementation patterns: partial response model, custom deep merge, StateManager/ActionDispatcher singleton architecture, array merge strategies, domain-scoped signals.
- `map-generation-research.md` — Three map generation approaches (Option A/B/C), Simplex noise detail, hex grid coordinate math.
- `medieval-peasant-history.md` — Historical research on medieval peasant status, guild system, mining rights, corvée, render-in-kind taxation. Design implications noted throughout.

### Decisions added from new sources

**ADR-002 updated: Godot 4 confirmed.** Architecture notes confirmed the Godot path and pinned version 4. Status changed from "open" to "decided." Serialization choice (`System.Text.Json`) and client architecture patterns (StateManager, ActionDispatcher) documented.

**ADR-005 created: Hex grid with axial coordinates.** Hex tiles selected over square grid (equidistant neighbors vs. ~41% diagonal distortion). Cube/axial (q, r) coordinate system. Reference: redblobgames.com/grids/hexagons.

**ADR-006 created: Map generation approach.** Start with Option A (Simplex Noise + climate bands); layer in Option B (Voronoi tectonics) and Option C (full atmospheric simulation) as needed.

**world-generation.md updated:** Hex tile shape and axial coordinates added. Map generation pipeline (Option A) documented. References to ADR-005 and ADR-006 added.

**frontend.md updated:** Godot 4 confirmed. StateManager and ActionDispatcher singleton patterns documented. Array merge strategies and domain-scoped signals added.

---

## Doc 2 → Doc 1 Reconciliation (2026-06-14)

Documentation populated from two design documents. Where they conflict, Doc 2 is authoritative.

### Changes and Clarifications Introduced by Doc 2

**Retainer terminology:** Doc 1 implied salaried military and administrative workers without naming them. Doc 2 formalizes the term "Retainers" and explicitly notes it replaces "direct employees" from earlier drafts.

**Three-tier labor model formalized:** Doc 2 explicitly names and defines the three tiers — Peasants (aggregate pool per tile), Specialists (named individual records), Retainers (named individual records) — with a formal comparison table covering tracking, compensation, output, consequences, and player control.

**Six-phase tick sequence:** Doc 1 described a tick-based economy at a high level. Doc 2 formalizes the six-phase sequence (Production, Taxes & Salary, Consumption, List for Sale, Retainer Requisition, Purchases) with precise pop behavior at each phase.

**Goods tax constraint added:** Doc 2 specifies the goods tax can only be set to a good the peasant population is actually capable of producing on their current tile. Doc 1 did not specify this constraint.

**Specialist tax is on finished output:** Doc 2 clarifies that specialists pay goods tax on their finished output, not raw inputs. Doc 1 implied but did not state this explicitly.

**Wealth-ordered round-robin in Phase 6:** Doc 2 specifies that market purchases in Phase 6 proceed via a wealth-ordered round-robin, with the lord in first position. Doc 1 described market purchasing without this mechanism.

**Merchant system fully specified:** Doc 2 provides the complete merchant model including the trade ledger, routing instructions, phase participation, and taxation structure (host city market fee, home city charter fee, staple rights). Doc 1 referenced trade routes and prestige-as-trade-range but did not specify the merchant mechanic.

**Retainer luxury consumption is mandatory:** Doc 2 specifies that luxury consumption by Knights and Generals is part of their upkeep, not optional.
