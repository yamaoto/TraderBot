using System.Diagnostics;
using TraderBot.EmailListener.Commands;

namespace TraderBot.EmailListener.HostedServices;

public class SchedulerHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SchedulerHostedService> _logger;
    public SchedulerHostedService(IServiceProvider serviceProvider, ILogger<SchedulerHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting SchedulerHostedService background service");
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(100, cancellationToken);
            using var scope = _serviceProvider.CreateScope();
            var collectProcessCommand = scope.ServiceProvider.GetRequiredService<CollectProcessCommand>();
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                await collectProcessCommand.CollectProcessAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("Scheduler job error. Original error: {Error}", e);
            }
            stopWatch.Stop();
            _logger.LogInformation("Job execution time {ElapsedMilliseconds} ms ", stopWatch.ElapsedMilliseconds);
        }
    }
}