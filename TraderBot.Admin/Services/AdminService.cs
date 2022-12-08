using System.Globalization;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TraderBot.AdminProto;
using TraderBot.RavenDb.OrderDomain;

namespace TraderBot.Admin.Services;

public class AdminService : AdminGrpc.AdminGrpcBase
{
    private readonly IOrderDal _orderDal;

    public AdminService(
        IOrderDal orderDal
        )
    {
        _orderDal = orderDal;
    }

    public override async Task<GetOrdersResponse> GetOrders(Empty request, ServerCallContext context)
    {
        var orders = (await _orderDal.GetOrdersAsync()).Select(s => new Order()
        {
            CreatedAt = s.OrderCreatedAt.ToString("O"),
            From = s.CopyFrom,
            Id = s.Id,
            OrderSide = s.OrderType,
            Price = s.Price.ToString("F5", CultureInfo.InvariantCulture),
            Quantity = s.Quantity.ToString("F5", CultureInfo.InvariantCulture),
            TradingSymbol = s.Symbol,
            Status = s.Status.ToString("F")
        });
        return new GetOrdersResponse
        {
            Orders = { orders }
        };
    }
}