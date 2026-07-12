namespace Demesne.Server.Models;

// REQ-205: all action POST responses use this envelope.
// Delta is sparse — only populated fields are considered changed by the client.
// ServerTimestamp reflects action completion time, which is appropriate for client-side delta ordering.
public record ActionResponse<T>(
    bool Success,
    string? ErrorCode,
    string? ErrorMessage,
    T? Delta,
    long ServerTimestamp,
    string ActionId) where T : class
{
    public static ActionResponse<T> Ok(T delta, string actionId) =>
        new(true, null, null, delta, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), actionId);

    public static ActionResponse<T> Fail(string errorCode, string errorMessage, string actionId) =>
        new(false, errorCode, errorMessage, null, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), actionId);
}
