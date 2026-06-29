using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Demesne.Server.Tiles;

[ApiController]
[Route("tiles")]
[Authorize]
[EnableRateLimiting("api")]
public class TileController : ControllerBase
{
    // Hardcoded starter tile for Milestone 1 walking skeleton.
    // Replaced by procedural generation in Milestone 4 (REQ-001).
    private static readonly TileSnapshot StarterTile = new(
        TileId: "00000000-0000-0000-0000-000000000001",
        Q: 0,
        R: 0,
        Altitude: 0.45,
        Temperature: 0.62,
        Precipitation: 0.55,
        ArablePercentage: 0.70,
        TerrainType: "Meadow",
        Resources:
        [
            new TileResource("Timber", 150),
            new TileResource("Stone", 80),
        ]);

    [HttpGet("{q:int}/{r:int}")]
    public IActionResult GetTile(int q, int r)
    {
        if (q != 0 || r != 0)
            return NotFound(new { error = "Only tile (0,0) exists in Milestone 1." });

        return Ok(StarterTile);
    }
}
