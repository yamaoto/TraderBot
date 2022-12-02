using Grpc.Core;
using TraderBot.Abstractions;
using TraderBot.OrderController.Commands;
using TraderBot.OrderControllerProto;

namespace TraderBot.OrderController.Services;

public class OrderControllerService : OrderControllerGrpc.OrderControllerGrpcBase
{
    private readonly ILogger<OrderControllerService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public OrderControllerService(
        ILogger<OrderControllerService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        var createOrderCommand = _serviceProvider.GetRequiredService<CreateOrder>();
        try
        {
            await createOrderCommand.CreateOrderAsync(request);
            return new CreateOrderResponse
            {
                Result = true,
                ErrorCode = "",
                ErrorMessage = ""
            };
        }
        catch (AppException e)
        {
            _logger.LogError(e, e.Message);
            return new CreateOrderResponse
            {
                Result = false,
                ErrorCode = e.ErrorCode,
                ErrorMessage = e.Message
            };
        }
        catch (Exception e)
        {
            const string errorMessage = "Error when creating new order. See exception message below.";
            _logger.LogError(e, errorMessage);
            return new CreateOrderResponse
            {
                Result = false,
                ErrorCode = "UNKNOWN",
                ErrorMessage = errorMessage + e
            };
        }
    }
}