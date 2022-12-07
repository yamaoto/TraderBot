using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TraderBot.AdminProto;
using TraderBot.RavenDb.OrderDomain;

namespace TraderBot.Admin.Services;

public class AdminService : AdminGrpc.AdminGrpcBase
{
    private readonly ILogger<AdminService> _logger;
    private readonly IOrderDal _orderDal;

    public AdminService(
        ILogger<AdminService> logger,
        IOrderDal orderDal)
    {
        _logger = logger;
        _orderDal = orderDal;
    }

    public override async Task<GetOrdersResponse> GetOrders(Empty request, ServerCallContext context)
    {
        var orders = (await _orderDal.GetOrdersAsync()).Select(s => new Order());
        return new GetOrdersResponse
        {
            Orders = { orders }
        };
    }
}