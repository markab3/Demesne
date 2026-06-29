using Godot;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Demesne.Client.Autoloads;

public partial class ActionDispatcher : Node
{
    // HttpClient is safe to reuse across requests. In Godot WASM the runtime maps
    // HttpClient to the browser's fetch API, so socket lifetime is browser-managed —
    // IHttpClientFactory patterns from server-side .NET do not apply here.
    private readonly HttpClient _http = new();
    private string _baseUrl = "http://localhost:5000";

    public override void _Ready()
    {
        _baseUrl = ProjectSettings.GetSetting("demesne/server_url", "http://localhost:5000").AsString();
    }

    // Sends an action to the server and merges the returned delta into StateManager.
    // Returns true on success, false on auth failure, validation failure, or network error.
    public async Task<bool> DispatchAsync(string endpoint, object payload)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/{endpoint}");
            request.Content = JsonContent.Create(payload);

            var token = GetNode<TokenStore>("/root/TokenStore").GetToken();
            if (token is not null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);
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
