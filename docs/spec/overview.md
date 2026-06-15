# Overview

## What

**Demesne: Trade & Tribute** — a browser-based multiplayer city-building game set in a medieval fantasy world. Players grow cities on a large open-world map, managing resources, specialists, diplomacy, and military forces.

## Design Philosophy

Economic specialization and political strategy are rewarded over raw military dominance. Asymmetric starting terrain makes trade feel necessary rather than optional. No two cities begin with identical natural specializations.

## Core Loop

- Claim and develop tiles to produce raw materials
- Attract and manage specialists to convert raw materials into finished goods
- Trade with other cities to acquire goods your terrain cannot supply
- Generate prestige, authority, and happiness to sustain and expand your city
- Engage in diplomacy and, when necessary, military action

## Instance Lifecycle

| Phase | Description |
|---|---|
| Lobby | Players join, select starting position, activity window advertised |
| Early Game | Expansion, initial specialization, first trade routes |
| Mid Game | Political blocs forming, vassal relationships, luxury chains online |
| Late Game | Map largely claimed, conflicts over high-value tiles, prestige competition |
| End Condition | Triggered by prestige threshold, map control percentage, or fixed session count |
| Postgame | Frozen permanently, viewable, stats available |

## Activity Window

The game runs during a shared 8-hour activity window per day. All players in a game instance share the same window. Outside the window the game is fully frozen — no ticks are processed, no military movement occurs, no territory can be lost. Players self-select into instances matching their availability.

## Technology Stack

- **Client:** Godot 4 (C#) exported to Web
- **Backend:** ASP.NET Core WebAPI
- **Real-time:** SignalR
- **Economy:** Server-side tick processor (one tick per real minute during the activity window)
