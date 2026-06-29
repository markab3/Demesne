using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Npgsql;

namespace Demesne.Server.State;

[ApiController]
[Route("state")]
[Authorize]
[EnableRateLimiting("api")]
public class StateController : ControllerBase
{
    private readonly NpgsqlDataSource _db;
    private readonly Tick.TickService _tick;

    public StateController(NpgsqlDataSource db, Tick.TickService tick)
    {
        _db = db;
        _tick = tick;
    }

    // Full state sync — called on login and reconnect (REQ-204: fog of war applied server-side).
    // Returns only the data the player is entitled to see.
    [HttpGet]
    public async Task<IActionResult> GetState()
    {
        var playerId = User.FindFirstValue("sub");
        if (playerId is null || !Guid.TryParse(playerId, out var playerGuid))
            return Unauthorized();

        CitySnapshot? city = null;

        await using var cmd = _db.CreateCommand(
            "SELECT city_id, name, prestige, authority, authority_budget, treasury, happiness, crime_pool " +
            "FROM cities WHERE owner_id = $1 LIMIT 1");
        cmd.Parameters.AddWithValue(playerGuid);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            city = new CitySnapshot(
                reader.GetGuid(0).ToString(),
                reader.GetString(1),
                reader.GetDouble(2),
                reader.GetDouble(3),
                reader.GetDouble(4),
                reader.GetInt32(5),
                reader.GetDouble(6),
                reader.GetDouble(7));
        }

        return Ok(new StateSnapshot(city, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), _tick.GameTick));
    }
}
