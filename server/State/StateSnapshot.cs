namespace Demesne.Server.State;

public record CitySnapshot(
    string CityId,
    string Name,
    double Prestige,
    double Authority,
    double AuthorityBudget,
    int Treasury,
    double Happiness,
    double CrimePool);

// Full state returned by GET /state on login or reconnect (REQ-204: fog of war applied server-side).
// IsInGame is false when the player is authenticated but has not yet been assigned a city.
// Additional fields (tiles, stockpile, etc.) will be added as milestones introduce them.
public record StateSnapshot(
    bool IsInGame,
    CitySnapshot? City,
    long ServerTimestamp,
    int GameTick);
