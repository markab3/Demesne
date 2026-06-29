namespace Demesne.Server.Tiles;

public record TileResource(string GoodType, double Quantity);

public record TileSnapshot(
    string TileId,
    int Q,
    int R,
    double Altitude,
    double Temperature,
    double Precipitation,
    double ArablePercentage,
    string TerrainType,
    IReadOnlyList<TileResource> Resources);
