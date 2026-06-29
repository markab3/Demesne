using Demesne.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Demesne.Server.Tick;

// Fires once per real minute. One tick = one game day (REQ-090).
// Activity window enforcement is deferred; the skeleton fires unconditionally.
public class TickService : BackgroundService
{
    private readonly IHubContext<GameHub> _hub;
    private readonly ILogger<TickService> _logger;
    private int _gameTick = 0;

    public int GameTick => _gameTick;

    public TickService(IHubContext<GameHub> hub, ILogger<TickService> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        while (await timer.WaitForNextTickAsync(ct))
            await ProcessTickAsync(ct);
    }

    private async Task ProcessTickAsync(CancellationToken ct)
    {
        _logger.LogInformation("Tick {Tick} starting", _gameTick + 1);

        // Six phases in order (REQ-011). Bodies filled in by Milestone 5.
        await Phase1ProductionAsync(ct);
        await Phase2TaxesAndSalaryAsync(ct);
        await Phase3ConsumptionAsync(ct);
        await Phase4ListForSaleAsync(ct);
        await Phase5RetainerRequisitionAsync(ct);
        await Phase6PurchasesAsync(ct);

        var newTick = Interlocked.Increment(ref _gameTick);
        _logger.LogInformation("Tick {Tick} complete", newTick);

        await _hub.Clients.All.SendAsync(
            "TickDelta",
            new { tick = newTick, serverTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
            ct);
    }

    private Task Phase1ProductionAsync(CancellationToken ct) => Task.CompletedTask;
    private Task Phase2TaxesAndSalaryAsync(CancellationToken ct) => Task.CompletedTask;
    private Task Phase3ConsumptionAsync(CancellationToken ct) => Task.CompletedTask;
    private Task Phase4ListForSaleAsync(CancellationToken ct) => Task.CompletedTask;
    private Task Phase5RetainerRequisitionAsync(CancellationToken ct) => Task.CompletedTask;
    private Task Phase6PurchasesAsync(CancellationToken ct) => Task.CompletedTask;
}
