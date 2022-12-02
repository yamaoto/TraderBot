namespace TraderBot.RavenDb.OrderDomain;

public interface IOrderDal
{
    Task<OrderRecord?> FindNewOrDraftOrderAsync(string symbol, string orderType, string copyFrom, decimal price,
        CancellationToken cancellationToken = default);

    Task<OrderRecord> GetOrderAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<OrderRecord>> GetOrdersAsync(CancellationToken cancellationToken = default);

    Task UpsertOrderAsync(OrderRecord order, CancellationToken cancellationToken = default);
}