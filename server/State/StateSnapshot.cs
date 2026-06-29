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
// Additional fields (tiles, stockpile, etc.) will be added as milestones introduce them.
public record StateSnapshot(
    CitySnapshot? City,
    long ServerTimestamp,
    int GameTick);
