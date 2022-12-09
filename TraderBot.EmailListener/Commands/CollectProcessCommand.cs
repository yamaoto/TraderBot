using TraderBot.EmailListener.Infrastructure;
using TraderBot.OrderControllerProto;
using TraderBot.TypesProto;

namespace TraderBot.EmailListener.Commands;

public class CollectProcessCommand
{
    private readonly ClosePositionHandler _closePositionHandler;
    private readonly ILogger<CollectProcessCommand> _logger;
    private readonly EmailChannel _emailChannel;
    private readonly OpenPositionHandler _openPositionHandler;
    private readonly OrderControllerGrpc.OrderControllerGrpcClient _orderControllerGrpcClient;

    public CollectProcessCommand(
        EmailChannel emailChannel,
        OpenPositionHandler openPositionHandler,
        ClosePositionHandler closePositionHandler,
        OrderControllerGrpc.OrderControllerGrpcClient orderControllerGrpcClient,
        ILogger<CollectProcessCommand> logger
    )
    {
        _emailChannel = emailChannel;
        _openPositionHandler = openPositionHandler;
        _closePositionHandler = closePositionHandler;
        _orderControllerGrpcClient = orderControllerGrpcClient;
        _logger = logger;
    }

    public async Task CollectProcessAsync()
    {
        var messages = _emailChannel.ChannelReader.ReadAllAsync();
        await foreach (var message in messages)
        {
            if (_openPositionHandler.IsApplicable(message))
            {
                try
                {
                    var openParameters = _openPositionHandler.ParseOpenPosition(message);
                    // TODO: Add InMemory cache for message deduplication
                    var response = await _orderControllerGrpcClient.CreateOrderAsync(new CreateOrderRequest
                    {
                        From = openParameters.From,
                        OrderSide = openParameters.OpeningSide.Value == "SELL" ? OrderSideType.Sell : OrderSideType.Buy,
                        Price = openParameters.Price.ConvertToTypesProtoDecimal(),
                        TradingSymbol = openParameters.TradingSymbol.Value,
                        Mailbox = message.Mailbox
                    });
                    if (!response.Result)
                        _logger.LogError(
                            "Failed when trying call OrderController for creating new order. Code: {ErrorCode}  message {ErrorMessage}",
                            response.ErrorCode, response.ErrorMessage);
                }
                catch (Exception e)
                {
                    _logger.LogError(
                        "Unknown error occured when handling open position message. Original error: {Error}",
                        e);
                }
            }
            else if (_closePositionHandler.IsApplicable(message))
            {
                try
                {
                    var closeParameters = _closePositionHandler.ParseClosePosition(message);
                    // TODO: Handle close position
                }
                catch (Exception e)
                {
                    _logger.LogError(
                        "Unknown error occured when handling close position message. Original error: {Error}",
                        e);
                }
            }
        }
    }
}