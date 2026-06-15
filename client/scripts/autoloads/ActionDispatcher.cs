using Godot;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Demesne.Client.Autoloads;

public partial class ActionDispatcher : Node
{
    private readonly HttpClient _http = new();
    private string _baseUrl = "http://localhost:5000";

    public override void _Ready()
    {
        _baseUrl = ProjectSettings.GetSetting("demesne/server_url", "http://localhost:5000").AsString();
    }

    // Sends an action to the server and merges the returned delta into StateManager.
    // Returns true on success, false on validation failure or network error.
    public async Task<bool> DispatchAsync(string endpoint, object payload)
    {
        try
        {
            var response = await _http.PostAsJsonAsync($"{_baseUrl}/{endpoint}", payload);
            if (!response.IsSuccessStatusCode)
                return false;

            var body = await response.Content.ReadFromJsonAsync<JsonObject>();
            if (body?["success"]?.GetValue<bool>() != true)
                return false;

            if (body["delta"] is JsonObject delta)
                GetNode<StateManager>("/root/StateManager").MergePartial(delta);

            return true;
        }
        catch (Exception e)
        {
            GD.PrintErr($"[ActionDispatcher] {endpoint}: {e.Message}");
            return false;
        }
    }
}
