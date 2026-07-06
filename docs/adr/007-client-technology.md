1|# 007-Client Technology: Blazor WebAssembly

## Situation

The current Godot 4 client implementation faces limitations for web deployment including lack of WebAssembly export for C# projects and dependency on proprietary Godot engines. Browser-based deployment is required for Demesne's target platform.

## Options Considered

1. **Evergine** - Web-focused engine but limited third-party tooling and smaller community
2. **Blazor WebAssembly + SkiaSharp** - Direct .NET compilation to WebAssembly with mature .NET ecosystem
3. **custom WebGL renderer** - More control but requires significant implementation effort

## Decision

Adopt Blazor WebAssembly as the new client frontend framework. Key advantages:
- Fully leverages existing C# skillset
- SkiaSharp provides native 2D rendering capabilities
- Solid .NET interoperability story
- Server hosting as pure ASP.NET Core backend
- Progressive Web App (PWA) capabilities

## Consequences

- Full rewrite of client components (ActionDispatcher/StateManager)
- Potential performance delta (initial speed tests required)
- Tight coupling between C# backend and client
- steeper learning curve for Blaze/Razor syntax
- Requires WebAssembly native compilation tooling

## Status

Decided - 2026-07-05

## Index

ADR | Title | Status |
---|---|---|
7 | [007](007-client-technology.md) | Decided |
