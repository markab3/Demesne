using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Npgsql;

namespace Demesne.Server.Auth;

[ApiController]
[Route("auth")]
[EnableRateLimiting("auth")]
public class AuthController : ControllerBase
{
    private readonly NpgsqlDataSource _db;
    private readonly TokenService _tokens;

    public AuthController(NpgsqlDataSource db, TokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { error = "Username and password are required." });

        if (req.Username.Length > 100)
            return BadRequest(new { error = "Username must be 100 characters or fewer." });

        if (req.Password.Length < 8)
            return BadRequest(new { error = "Password must be at least 8 characters." });

        var hash = BCrypt.Net.BCrypt.HashPassword(req.Password);

        await using var cmd = _db.CreateCommand(
            "INSERT INTO players (username, password_hash) VALUES ($1, $2) RETURNING player_id");
        cmd.Parameters.AddWithValue(req.Username);
        cmd.Parameters.AddWithValue(hash);

        try
        {
            var playerId = (Guid)(await cmd.ExecuteScalarAsync())!;
            var token = _tokens.GenerateToken(playerId.ToString(), req.Username);
            return Ok(new AuthResponse(token, playerId.ToString(), req.Username));
        }
        catch (PostgresException ex) when (ex.SqlState == "23505") // unique_violation
        {
            return Conflict(new { error = "Username already taken." });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { error = "Username and password are required." });

        await using var cmd = _db.CreateCommand(
            "SELECT player_id, password_hash FROM players WHERE username = $1");
        cmd.Parameters.AddWithValue(req.Username);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return Unauthorized(new { error = "Invalid credentials." });

        var playerId = reader.GetGuid(0).ToString();
        var passwordHash = reader.GetString(1);

        if (!BCrypt.Net.BCrypt.Verify(req.Password, passwordHash))
            return Unauthorized(new { error = "Invalid credentials." });

        var token = _tokens.GenerateToken(playerId, req.Username);
        return Ok(new AuthResponse(token, playerId, req.Username));
    }
}
