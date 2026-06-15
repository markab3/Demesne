using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using System.Text.Json.Nodes;
using Demesne.Client.Autoloads;

namespace Demesne.Client;

public partial class Main : Node
{
    private HubConnection? _hub;

    public override void _Ready()
    {
        _ = ConnectAsync();
    }

    private async Task ConnectAsync()
    {
        var serverUrl = ProjectSettings.GetSetting("demesne/server_url", "http://localhost:5000").AsString();

        _hub = new HubConnectionBuilder()
            .WithUrl($"{serverUrl}/hubs/game")
            .WithAutomaticReconnect()
            .Build();

        _hub.On<JsonElement>("Welcome", msg =>
            GD.Print($"[SignalR] {msg}"));

        _hub.On<JsonElement>("TickDelta", delta =>
        {
            if (JsonNode.Parse(delta.GetRawText()) is JsonObject obj)
                GetNode<StateManager>("/root/StateManager").MergePartial(obj);
        });

        try
        {
            await _hub.StartAsync();
            GD.Print("[SignalR] Connected to game server");
        }
        catch (Exception e)
        {
            GD.PrintErr($"[SignalR] Connection failed: {e.Message}");
        }
    }

    public override void _ExitTree()
    {
        _ = _hub?.DisposeAsync().AsTask();
    }
}
