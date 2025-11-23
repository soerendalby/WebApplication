using Microsoft.Extensions.Hosting;

namespace WebApplication.Services.Hosted;

public class ReminderWorker : BackgroundService
{
    private readonly ILogger<ReminderWorker> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

    public ReminderWorker(ILogger<ReminderWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ReminderWorker started");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // TODO: Implement reminder/expiry logic in later phase
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReminderWorker loop");
            }

            try
            {
                await Task.Delay(_interval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
        }
        _logger.LogInformation("ReminderWorker stopping");
    }
}
