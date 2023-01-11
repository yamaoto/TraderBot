using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;

namespace TraderBot.RavenDb;

public class RavenDbHealthChecks : IHealthCheck
{
    private readonly IDocumentStore _documentStore;
    private readonly IOptions<RavenDbOptions> _options;

    public RavenDbHealthChecks(
        IDocumentStore documentStore,
        IOptions<RavenDbOptions> options
    )
    {
        _documentStore = documentStore;
        _options = options;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new())
    {
        if (_options.Value.UseStub)
        {
            return new HealthCheckResult(HealthStatus.Healthy);
        }

        try
        {
            await _documentStore.Maintenance.ForDatabase(_options.Value.DatabaseName)
                .SendAsync(new GetStatisticsOperation(), cancellationToken);
            return new HealthCheckResult(HealthStatus.Healthy);
        }
        catch (Exception)
        {
            return new HealthCheckResult(HealthStatus.Unhealthy, "RavenDb is unavailable");
        }
    }
}