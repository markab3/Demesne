using Demesne.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Demesne.Server.Tick;

// Fires once per real minute. One tick = one game day (REQ-090).
// Activity window enforcement is deferred; the skeleton fires unconditionally.
public class TickService : BackgroundService, ITickState
{
    private readonly IHubContext<GameHub> _hub;
    private readonly ILogger<TickService> _logger;

    // volatile so plain reads in GameTick have a memory barrier on non-x86 architectures (B2).
    // Writes always go through Interlocked.Increment which already implies a full barrier.
    private volatile int _gameTick = 0;

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
        {
            // B4: catch all non-cancellation exceptions so a single bad tick does not
            // permanently kill the background service for the lifetime of the process.
            try
            {
                await ProcessTickAsync(ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Tick {Tick} failed; skipping tick", _gameTick);
            }
        }
    }

    private async Task ProcessTickAsync(CancellationToken ct)
    {
        // B3: increment first so the captured tick number is used consistently
        // across the log and the TickDelta push, with no racy read-before-increment.
        var tick = Interlocked.Increment(ref _gameTick);
        _logger.LogInformation("Tick {Tick} starting", tick);

        // Six phases in order (REQ-011). Bodies filled in by Milestone 5.
        await Phase1ProductionAsync(ct);
        await Phase2TaxesAndSalaryAsync(ct);
        await Phase3ConsumptionAsync(ct);
        await Phase4ListForSaleAsync(ct);
        await Phase5RetainerRequisitionAsync(ct);
        await Phase6PurchasesAsync(ct);

        _logger.LogInformation("Tick {Tick} complete", tick);

        await _hub.Clients.All.SendAsync(
            "TickDelta",
            new { tick, serverTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
            ct);
    }

    private Task Phase1ProductionAsync(CancellationToken ct) => Task.CompletedTask;
    private Task Phase2TaxesAndSalaryAsync(CancellationToken ct) => Task.CompletedTask;
    private Task Phase3ConsumptionAsync(CancellationToken ct) => Task.CompletedTask;
    private Task Phase4ListForSaleAsync(CancellationToken ct) => Task.CompletedTask;
    private Task Phase5RetainerRequisitionAsync(CancellationToken ct) => Task.CompletedTask;
    private Task Phase6PurchasesAsync(CancellationToken ct) => Task.CompletedTask;
}
