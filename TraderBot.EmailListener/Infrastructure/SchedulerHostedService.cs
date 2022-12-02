using System.Diagnostics;
using TraderBot.EmailListener.Commands;

namespace TraderBot.EmailListener.Infrastructure;

public class SchedulerHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private ILogger<SchedulerHostedService> _logger;
    public SchedulerHostedService(IServiceProvider serviceProvider, ILogger<SchedulerHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(3000, stoppingToken);
            using var scope = _serviceProvider.CreateScope();
            var collectProcessCommand = scope.ServiceProvider.GetRequiredService<CollectProcessCommand>();
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                await collectProcessCommand.CollectProcessAsync(DateTime.Today.AddHours(-1), "");
            }
            catch (Exception e)
            {
                _logger.LogError("Scheduler job error. Original error: {error}", e);
            }
            stopWatch.Stop();
            _logger.LogInformation("Job execution time {time} ms ", stopWatch.ElapsedMilliseconds);
        }
    }
}