using Raven.Client.Documents;
using TraderBot.Abstractions;

namespace TraderBot.RavenDb.OrderDomain;

public class RavenOrderDal : IOrderDal
{
    private readonly IDocumentStore _documentStore;

    public RavenOrderDal(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<OrderRecord?> FindNewOrDraftOrderAsync(string symbol, string orderType, string copyFrom,
        decimal price, CancellationToken cancellationToken = default)
    {
        using var session = _documentStore.OpenAsyncSession();
        var orders = await session.Query<OrderRecord>()
            .Where(o => o.Symbol == symbol)
            .Where(o => o.OrderType == orderType)
            .Where(o => o.CopyFrom == copyFrom)
            .Where(o => o.Status == OrderStatus.Created || o.Status == OrderStatus.No)
            .Where(o => o.Price == price)
            // .Select(p =>
            //     RavenQuery.TimeSeries(p, "DateTime")
            //         .FromLast(g => g.Hours(24)))
            .Where(w => w.OrderCreatedAt > DateTime.UtcNow.AddHours(-24))
            .ToListAsync(cancellationToken);
        if (orders.Count > 1)
        {
            var ids = string.Join(' ', orders.Select(s => s.Id));
            throw new AppException("DB_COLLISION",
                $"Database have more than one record for similiar parameters. Ids: {ids}");
        }

        return orders.FirstOrDefault();
    }

    public async Task<OrderRecord> GetOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var session = _documentStore.OpenAsyncSession();
        var order = await session.LoadAsync<OrderRecord>(id.ToString(), cancellationToken);
        return order;
    }

    public async Task<IEnumerable<OrderRecord>> GetOrdersAsync(CancellationToken cancellationToken = default)
    {
        using var session = _documentStore.OpenAsyncSession();
        var orders = await session.Query<OrderRecord>()
            .OrderByDescending(o => o.OrderCreatedAt)
            .ToListAsync(cancellationToken);
        return orders;
    }

    public async Task UpsertOrderAsync(OrderRecord order, CancellationToken cancellationToken = default)
    {
        using var session = _documentStore.OpenAsyncSession();
        await session.StoreAsync(order, order.Id, cancellationToken);
        await session.SaveChangesAsync(cancellationToken);
    }
}