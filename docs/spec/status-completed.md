# Completed Work

Items moved here from `status.md` once marked done, so the active status file stays short. Append-only — do not delete entries.

## Foundation — Infrastructure (completed)

- [x] ASP.NET Core WebAPI project scaffolded, runs locally (`dotnet run`)
- [x] Database schema initialized from `docs/design/data-model.md`
- [x] SignalR hub configured; client can connect and receive a server push
- [x] Godot 4 (C#) project scaffolded, runs in-editor against local API
- [x] `StateManager` autoload: holds display state, exposes `MergePartial`, emits domain-scoped signals
- [x] `ActionDispatcher` autoload: wraps all HTTP POST calls with typed methods
- [x] Player auth: register, login, session token issued and validated on subsequent requests
- [x] Full-state sync on login/reconnect
- [x] Delta response shape: action responses return sparse changed-field objects (REQ-205)
- [x] Tick processor skeleton: fires once per real minute during activity window, six empty phase stubs in order
