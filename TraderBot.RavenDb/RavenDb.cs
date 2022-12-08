using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        var store = new DocumentStore
        {
            Urls = ravenDbConfiguration!.Urls.ToArray(),
            Database = ravenDbConfiguration.DatabaseName
        }.Initialize();
        services.AddSingleton(store);
        services.AddTransient<IOrderDal, RavenOrderDal>();
        services.AddTransient<IMailBoxDal, RavenMailBoxDal>();

        await EnsureDatabaseExistsAsync(store, ravenDbConfiguration);
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
            try
            {
                await store.Maintenance.Server.SendAsync(new CreateDatabaseOperation(new DatabaseRecord(database)));
            }
            catch (ConcurrencyException)
            {
            }
        }
    }
}