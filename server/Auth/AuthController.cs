using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Npgsql;

namespace Demesne.Server.Auth;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    // S5: pre-computed once at class init for timing-safe username enumeration defence.
    // BCrypt.Verify is called against this even when the username does not exist so that
    // a missed-username path takes the same time as a wrong-password path.
    private static readonly string _dummyHash =
        BCrypt.Net.BCrypt.HashPassword("timing-guard", workFactor: 11);

    private readonly NpgsqlDataSource _db;
    private readonly TokenService _tokens;

    public AuthController(NpgsqlDataSource db, TokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    [HttpPost("register")]
    [EnableRateLimiting("auth")] // D2: per-method so future /auth/* endpoints choose their own policy
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { error = "Username and password are required." });

        if (req.Username.Length > 100)
            return BadRequest(new { error = "Username must be 100 characters or fewer." });

        if (req.Password.Length < 8)
            return BadRequest(new { error = "Password must be at least 8 characters." });

        if (req.Password.Length > 72)
            return BadRequest(new { error = "Password must be 72 characters or fewer." });

        var hash = BCrypt.Net.BCrypt.HashPassword(req.Password, workFactor: 11);

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
    [EnableRateLimiting("auth")] // D2: per-method
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { error = "Username and password are required." });

        await using var cmd = _db.CreateCommand(
            "SELECT player_id, password_hash FROM players WHERE username = $1 LIMIT 1");
        cmd.Parameters.AddWithValue(req.Username);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            // S5: constant-time branch — always run BCrypt even on a miss so response
            // time does not reveal whether the username exists.
            BCrypt.Net.BCrypt.Verify(req.Password, _dummyHash);
            return Unauthorized(new { error = "Invalid credentials." });
        }

        var playerId = reader.GetGuid(0).ToString();
        var passwordHash = reader.GetString(1);

        if (!BCrypt.Net.BCrypt.Verify(req.Password, passwordHash))
            return Unauthorized(new { error = "Invalid credentials." });

        var token = _tokens.GenerateToken(playerId, req.Username);
        return Ok(new AuthResponse(token, playerId, req.Username));
    }
}
