using TraderBot.EmailListener.Infrastructure;
using TraderBot.OrderControllerProto;
using TraderBot.TypesProto;

namespace TraderBot.EmailListener.Commands;

public class CollectProcessCommand
{
    private readonly ClosePositionHandler _closePositionHandler;
    private readonly GetMessagesCommand _getMessagesCommand;
    private readonly ILogger<CollectProcessCommand> _logger;
    private readonly OpenPositionHandler _openPositionHandler;
    private readonly OrderControllerGrpc.OrderControllerGrpcClient _orderControllerGrpcClient;

    public CollectProcessCommand(
        GetMessagesCommand getMessagesCommand,
        OpenPositionHandler openPositionHandler,
        ClosePositionHandler closePositionHandler,
        OrderControllerGrpc.OrderControllerGrpcClient orderControllerGrpcClient,
        ILogger<CollectProcessCommand> logger
    )
    {
        _getMessagesCommand = getMessagesCommand;
        _openPositionHandler = openPositionHandler;
        _closePositionHandler = closePositionHandler;
        _orderControllerGrpcClient = orderControllerGrpcClient;
        _logger = logger;
    }

    public async Task CollectProcessAsync(DateTime lastSyncPoint, string lastProcessedEmail)
    {
        var messages = await _getMessagesCommand.GetMessagesAsync(lastSyncPoint, lastProcessedEmail);
        foreach (var message in messages)
            if (_openPositionHandler.IsApplicable(message))
            {
                var openParameters = _openPositionHandler.ParseOpenPosition(message);
                // TODO: Add InMemory cache for message deduplication
                var response = await _orderControllerGrpcClient.CreateOrderAsync(new CreateOrderRequest
                {
                    From = openParameters.From,
                    OrderSide = openParameters.OpeningSide.Value == "SELL" ? OrderSideType.Sell : OrderSideType.Buy,
                    Price = openParameters.Price.ConvertToTypesProtoDecimal(),
                    TradingSymbol = openParameters.TradingSymbol.Value
                });
                if (!response.Result)
                    _logger.LogError(
                        "Failed when trying call OrderController for creating new order. Code: {ErrorCode}  message {ErrorMessage}",
                        response.ErrorCode, response.ErrorMessage);
            }
            else if (_closePositionHandler.IsApplicable(message))
            {
                var closeParameters = _closePositionHandler.ParseClosePosition(message);
            }
    }
}