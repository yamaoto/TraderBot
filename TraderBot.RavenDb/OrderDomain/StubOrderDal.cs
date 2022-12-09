namespace TraderBot.RavenDb.OrderDomain;

public class StubOrderDal : IOrderDal
{
    private Dictionary<string, OrderRecord> _orders = new Dictionary<string, OrderRecord>();

    public Task<OrderRecord?> FindNewOrDraftOrderAsync(string symbol, string orderType, string copyFrom, decimal price,
        CancellationToken cancellationToken = default)
    {
        var orders = _orders.Values.Where(f =>
            f.Symbol == symbol && f.OrderType == orderType && f.CopyFrom == copyFrom && f.Price == price);
        return Task.FromResult(orders.FirstOrDefault());
    }

    public Task<OrderRecord> GetOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_orders[id.ToString()]);
    }

    public Task<IEnumerable<OrderRecord>> GetOrdersAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult((IEnumerable<OrderRecord>)_orders.Values);
    }

    public Task UpsertOrderAsync(OrderRecord order, CancellationToken cancellationToken = default)
    {
        _orders[order.Id] = order;
        return Task.CompletedTask;
    }
}