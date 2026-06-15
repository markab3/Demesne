# ADR-002: Client Technology

Status: Decided — Godot 4 (C#)

## Context

The game requires a 2D tile map renderer running in the browser. Two viable C# paths exist, both sharing the language with the ASP.NET Core backend. The architecture exploration session confirmed the Godot path and produced implementation patterns (see `docs/notes/architecture-approaches.md`).

## Options Evaluated

**Option A: Godot 4 (C#) Web Export** ✅ Selected
- Purpose-built 2D tilemap system
- Exports to web via WASM
- C# matches the backend stack — potential for shared types
- Architecture exploration produced concrete Godot patterns (StateManager, ActionDispatcher autoloaded singletons, domain-scoped signals)

**Option B: Blazor WASM + PixiJS**
- Pure C# web stack
- Tile map handled via JavaScript canvas library (PixiJS) through JS interop
- JS interop adds complexity at the rendering boundary
- Not pursued further

## Decision

**Godot 4, C#, exported to Web.**

## Consequences

- Serialization: `System.Text.Json` / `JsonNode` (no Newtonsoft dependency)
- Client architecture: autoloaded singleton pattern (StateManager + ActionDispatcher)
- Godot signals used for domain-scoped state change notifications
- Build tooling and asset pipeline follow Godot 4 conventions
- SignalR client wired into Godot via C# GodotSharp integration
