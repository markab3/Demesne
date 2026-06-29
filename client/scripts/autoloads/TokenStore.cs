using Godot;

namespace Demesne.Client.Autoloads;

// Holds the player's JWT between requests. Populated by the auth flow (Milestone 3).
// Read by ActionDispatcher (Authorization header) and Main (SignalR AccessTokenProvider).
public partial class TokenStore : Node
{
    private string? _token;

    public void SetToken(string token) => _token = token;
    public string? GetToken() => _token;
    public void ClearToken() => _token = null;
    public bool HasToken => _token is not null;
}
