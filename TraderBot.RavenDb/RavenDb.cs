using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using TraderBot.RavenDb.MailBoxDomain;
using TraderBot.RavenDb.OrderDomain;

namespace TraderBot.RavenDb;

public static class RavenDb
{
    public static async Task AddAndConfigureRavenDbAsync(this IServiceCollection services, IConfiguration configuration)
    {
        var ravenDbConfigurationSection = configuration.GetSection("RavenDb");
        services.Configure<RavenDbOptions>(ravenDbConfigurationSection);
        var ravenDbConfiguration = ravenDbConfigurationSection.Get<RavenDbOptions>();
        if (ravenDbConfiguration == null)
        {
            throw new InvalidOperationException("You must configure RavenDb section");
        }
        var store = new DocumentStore
        {
            Urls = new[] { ravenDbConfiguration.Endpoint },
            Database = ravenDbConfiguration.DatabaseName
        }.Initialize();
        services.AddSingleton(store);
        if (ravenDbConfiguration.UseStub)
        {
            
            services.AddSingleton<IOrderDal, StubOrderDal>();
            services.AddSingleton<IMailBoxDal, StubMailBoxDal>();
        }
        else
        {
            services.AddTransient<IOrderDal, RavenOrderDal>();
            services.AddTransient<IMailBoxDal, RavenMailBoxDal>();
        }

        await Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            }).ExecuteAsync(async () => await EnsureDatabaseExistsAsync(store, ravenDbConfiguration));
    }

    private static async Task EnsureDatabaseExistsAsync(IDocumentStore store, RavenDbOptions options)
    {
        var database = options.DatabaseName;

        try
        {
            await store.Maintenance.ForDatabase(database).SendAsync(new GetStatisticsOperation());
        }
        catch (DatabaseDoesNotExistException)
        {
            await store.Maintenance.Server.SendAsync(new CreateDatabaseOperation(new DatabaseRecord(database)));
        }
    }
}