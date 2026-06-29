using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Demesne.Client.Autoloads;

namespace Demesne.Client;

public partial class Main : Node
{
    private HubConnection? _hub;
    private readonly HttpClient _http = new();
    private string _baseUrl = "http://localhost:5000";

    // UI nodes built programmatically so no scene editor is needed
    private VBoxContainer? _loginContainer;
    private LineEdit? _usernameInput;
    private LineEdit? _passwordInput;
    private Label? _loginStatus;
    private VBoxContainer? _gameContainer;
    private Label? _tickLabel;
    private Label? _tileLabel;

    public override void _Ready()
    {
        _baseUrl = ProjectSettings.GetSetting("demesne/server_url", "http://localhost:5000").AsString();
        // BaseAddress must end with "/" for relative paths to resolve correctly.
        _http.BaseAddress = new Uri(_baseUrl.TrimEnd('/') + "/");

        BuildUi();
        GetNode<StateManager>("/root/StateManager").TickAdvanced += OnTickAdvanced;
        _ = ConnectHubAsync();
    }

    private void BuildUi()
    {
        _loginContainer = new VBoxContainer();
        AddChild(_loginContainer);

        _loginContainer.AddChild(new Label { Text = "Demesne — Walking Skeleton" });

        _usernameInput = new LineEdit { PlaceholderText = "Username" };
        _loginContainer.AddChild(_usernameInput);

        _passwordInput = new LineEdit { PlaceholderText = "Password", Secret = true };
        _loginContainer.AddChild(_passwordInput);

        var loginBtn = new Button { Text = "Login" };
        loginBtn.Pressed += () => _ = AuthAsync(register: false);
        _loginContainer.AddChild(loginBtn);

        var registerBtn = new Button { Text = "Register" };
        registerBtn.Pressed += () => _ = AuthAsync(register: true);
        _loginContainer.AddChild(registerBtn);

        _loginStatus = new Label();
        _loginContainer.AddChild(_loginStatus);

        _gameContainer = new VBoxContainer { Visible = false };
        AddChild(_gameContainer);

        _tickLabel = new Label { Text = "Game Tick: 0" };
        _gameContainer.AddChild(_tickLabel);

        _tileLabel = new Label { Text = "Tile (0,0): loading…" };
        _gameContainer.AddChild(_tileLabel);
    }

    private async Task AuthAsync(bool register)
    {
        if (_usernameInput is null || _passwordInput is null) return;
        var username = _usernameInput.Text.Trim();
        var password = _passwordInput.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            SetLoginStatus("Username and password required.");
            return;
        }

        try
        {
            var path = register ? "auth/register" : "auth/login";
            var resp = await _http.PostAsJsonAsync(path, new { username, password });

            var body = await resp.Content.ReadFromJsonAsync<JsonObject>();
            if (!resp.IsSuccessStatusCode)
            {
                SetLoginStatus(body?["error"]?.GetValue<string>() ?? "Request failed.");
                return;
            }

            var token = body?["token"]?.GetValue<string>();
            if (token is null) { SetLoginStatus("No token in response."); return; }

            GetNode<TokenStore>("/root/TokenStore").SetToken(token);
            await OnLoggedIn();
        }
        catch (Exception e)
        {
            SetLoginStatus($"Error: {e.Message}");
        }
    }

    private async Task OnLoggedIn()
    {
        if (_loginContainer is not null) _loginContainer.Visible = false;
        if (_gameContainer is not null) _gameContainer.Visible = true;

        // Hub connects with the token now stored in TokenStore
        if (_hub?.State != HubConnectionState.Connected)
            _ = ConnectHubAsync();

        await FetchTileAsync();
    }

    private async Task FetchTileAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "tiles/0/0");
            var token = GetNode<TokenStore>("/root/TokenStore").GetToken();
            if (token is not null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await _http.SendAsync(request);
            if (!resp.IsSuccessStatusCode)
            {
                SetTileText("Tile fetch failed.");
                return;
            }

            var tile = await resp.Content.ReadFromJsonAsync<JsonObject>();
            if (tile is null) { SetTileText("Empty tile response."); return; }

            var text = $"Tile ({tile["q"]?.GetValue<int>()}, {tile["r"]?.GetValue<int>()})\n" +
                       $"  Terrain:       {tile["terrainType"]?.GetValue<string>()}\n" +
                       $"  Altitude:      {tile["altitude"]?.GetValue<double>():F2}\n" +
                       $"  Temperature:   {tile["temperature"]?.GetValue<double>():F2}\n" +
                       $"  Precipitation: {tile["precipitation"]?.GetValue<double>():F2}\n" +
                       $"  Arable:        {tile["arablePercentage"]?.GetValue<double>():P0}";

            if (tile["resources"] is JsonArray resources)
            {
                text += "\n  Resources:";
                foreach (var r in resources.OfType<JsonObject>())
                    text += $"\n    {r["goodType"]?.GetValue<string>()}: {r["quantity"]?.GetValue<double>():F0}";
            }

            SetTileText(text);
        }
        catch (Exception e)
        {
            SetTileText($"Tile error: {e.Message}");
        }
    }

    private void OnTickAdvanced(int newTick)
    {
        if (_tickLabel is not null) _tickLabel.Text = $"Game Tick: {newTick}";
    }

    private void SetLoginStatus(string msg)
    {
        if (_loginStatus is not null) _loginStatus.Text = msg;
    }

    private void SetTileText(string msg)
    {
        if (_tileLabel is not null) _tileLabel.Text = msg;
    }

    private async Task ConnectHubAsync()
    {
        if (_hub is not null)
        {
            await _hub.DisposeAsync();
            _hub = null;
        }

        _hub = new HubConnectionBuilder()
            .WithUrl($"{_baseUrl}/hubs/game", opts =>
            {
                opts.AccessTokenProvider = () =>
                    Task.FromResult(GetNode<TokenStore>("/root/TokenStore").GetToken());
            })
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
