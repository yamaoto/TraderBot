using System.Text;
using Microsoft.Extensions.Options;
using TraderBot.Abstractions;
using TraderBot.BinanceConnectProto;
using TraderBot.OrderController.Infrastructure;
using TraderBot.OrderControllerProto;
using TraderBot.RavenDb.MailBoxDomain;
using TraderBot.RavenDb.OrderDomain;
using TraderBot.TypesProto;

namespace TraderBot.OrderController.Commands;

public class CreateOrder
{
    private readonly ILogger<CreateOrder> _logger;
    private readonly IOrderDal _orderDal;
    private readonly IMailBoxDal _mailBoxDal;
    private readonly IGetExchangeStepSize _getExchangeStepSize;
    private readonly SpotGrpc.SpotGrpcClient _spotServiceClient;
    private readonly ITelegramService _telegramService;
    private readonly IOptions<TradingOptions> _tradingOptions;

    public CreateOrder(
        IOrderDal orderDal,
        IMailBoxDal mailBoxDal,
        IGetExchangeStepSize getExchangeStepSize,
        SpotGrpc.SpotGrpcClient spotServiceClient,
        ITelegramService telegramService,
        IOptions<TradingOptions> tradingOptions,
        ILogger<CreateOrder> logger
    )
    {
        _orderDal = orderDal;
        _mailBoxDal = mailBoxDal;
        _getExchangeStepSize = getExchangeStepSize;
        _spotServiceClient = spotServiceClient;
        _telegramService = telegramService;
        _tradingOptions = tradingOptions;
        _logger = logger;
    }

    public async Task CreateOrderAsync(CreateOrderRequest request)
    {
        var mailBox = await _mailBoxDal.GetMailBoxAsync(request.Mailbox);
        if (mailBox == null || !mailBox.AllowedCopyFrom.Contains(request.From))
        {
            return;
        }
        var orderSide = request.OrderSide == OrderSideType.Sell ? "SELL" : "BUY";
        var price = request.Price.ConvertToRegularDecimal();
        var existingOrder = await _orderDal.FindNewOrDraftOrderAsync(request.TradingSymbol, orderSide,
            request.From, price);
        var id = Guid.NewGuid();
        var timestamp = DateTime.UtcNow;
        if (existingOrder != null)
        {
            // deduplication for more that one message delivery
            _logger.LogInformation("Duplicating order message delivery found");
            return;
        }

        var balance = await GetFuturesBalanceAsync(mailBox.Name);
        var tradingOptions = _tradingOptions.Value.GetOptionsForSymbol(request.TradingSymbol);
        var originalQuantity = balance * tradingOptions.Rate / price;

        var symbolInfo = await _getExchangeStepSize.GetExchangeSymbolInfoAsync(request.TradingSymbol);
        var quantity = Math.Floor(originalQuantity / symbolInfo.StepSize) * symbolInfo.StepSize;
        if (quantity < symbolInfo.MinimalQuantity)
        {
            quantity = symbolInfo.MinimalQuantity;
        }
        if (quantity > symbolInfo.MaximalQuantity)
        {
            quantity = symbolInfo.MaximalQuantity;
        }
        // save draft order record
        await UpsertOrderRecordAsync(request, id, timestamp, orderSide, quantity, OrderStatus.No);

        var openSpotRequest = new OpenSpotRequest
        {
            TradingSymbol = request.TradingSymbol,
            OrderSide = request.OrderSide,
            Price = request.Price,
            Quantity = quantity.ConvertToTypesProtoDecimal(),
            Mailbox = mailBox.Name
        };
        // TODO: Add external binance id for order
        var openSpotResponse = await _spotServiceClient.OpenSpotAsync(openSpotRequest);
        if (!openSpotResponse.Result)
        {
            var errorMessage =
                $"Error when trying open spot from BinanceConnect. Code: {openSpotResponse.ErrorCode} message: {openSpotResponse.ErrorMessage}";
            throw new AppException(openSpotResponse.ErrorCode, errorMessage);
        }

        // save successfully created order record
        await UpsertOrderRecordAsync(request, id, timestamp, orderSide, quantity, OrderStatus.Created);

        await NotifyByTelegramAsync(request, quantity);
    }

    private async Task UpsertOrderRecordAsync(CreateOrderRequest request, Guid id, DateTime timestamp, string orderSide,
        decimal quantity, OrderStatus status)
    {
        await _orderDal.UpsertOrderAsync(new OrderRecord
        {
            Id = id.ToString(),
            Mailbox = request.Mailbox,
            Symbol = request.TradingSymbol,
            CopyFrom = request.From,
            OrderCreatedAt = timestamp,
            OrderType = orderSide,
            Price = request.Price.ConvertToRegularDecimal(),
            Quantity = quantity,
            Status = status
        });
    }

    private async Task<decimal> GetFuturesBalanceAsync(string mailbox)
    {
        var balanceResponse = await _spotServiceClient.GetUsdtBalanceAsync(new GetUsdtBalanceRequest()
        {
            Mailbox = mailbox
        });
        if (!balanceResponse.Result)
        {
            var errorMessage =
                $"Error when trying get balance from BinanceConnect. Code: {balanceResponse.ErrorCode} message: {balanceResponse.ErrorMessage}";
            throw new AppException(balanceResponse.ErrorCode, errorMessage);
        }

        var balance = balanceResponse.Balance.ConvertToRegularDecimal();
        return balance;
    }

    private async Task NotifyByTelegramAsync(CreateOrderRequest request, decimal quantity)
    {
        var stringBuilder = new StringBuilder();
        var price = request.Price.ConvertToRegularDecimal();
        stringBuilder.Append($"Начинаем новый шорт {request.TradingSymbol} на {quantity} позиций * {price}");
        if (request.From != null) stringBuilder.Append($". Копируем у {request.From}");

        await _telegramService.SendMessageAsync(stringBuilder.ToString());
    }
}