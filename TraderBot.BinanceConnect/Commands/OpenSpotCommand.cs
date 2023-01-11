using Binance.Common;
using Microsoft.Extensions.Options;
using TraderBot.Abstractions;
using TraderBot.BinanceConnect.BinanceSupport;
using TraderBot.BinanceConnect.Infrastructure;
using TraderBot.BinanceConnectProto;
using TraderBot.RavenDb.MailBoxDomain;
using TraderBot.TypesProto;

namespace TraderBot.BinanceConnect.Commands;

public class OpenSpotCommand
{
    private readonly IMailBoxDal _mailBoxDal;
    private readonly HttpClient _httpClient;
    private readonly BinanceMetrics _binanceMetrics;
    private readonly IOptions<BinanceOptions> _options;

    public OpenSpotCommand(
        IMailBoxDal mailBoxDal,
        HttpClient httpClient,
        BinanceMetrics binanceMetrics,
        IOptions<BinanceOptions> options
    )
    {
        _mailBoxDal = mailBoxDal;
        _httpClient = httpClient;
        _binanceMetrics = binanceMetrics;
        _options = options;
    }

    public async Task<FuturesNewOrderResponse> OpenSpotAsync(OpenSpotRequest openSpotParameters)
    {
        var mailBox = await _mailBoxDal.GetMailBoxAsync(openSpotParameters.Mailbox);
        var baseUrl = _options.Value.UseTestnet ? "https://testnet.binancefuture.com" : "https://fapi.binance.com";
        var futureTrade = new FuturesTradeClient(_httpClient, baseUrl, mailBox.BinanceApiKey,
            new BinanceHmac(mailBox.BinanceApiSecret)
        );
        var newOrderResponse = await futureTrade.NewOrderAsync(openSpotParameters);
        var orderSide = openSpotParameters.OrderSide == OrderSideType.Buy ? "BUY" : "SELL";
        _binanceMetrics
            .GetHistogramMetricForSymbol(openSpotParameters.TradingSymbol, "quantity", new OrderSide(orderSide))
            .Observe(Convert.ToDouble(openSpotParameters.Quantity.ConvertToRegularDecimal()));
        _binanceMetrics
            .GetHistogramMetricForSymbol(openSpotParameters.TradingSymbol, "price", new OrderSide(orderSide))
            .Observe(Convert.ToDouble(openSpotParameters.Price.ConvertToRegularDecimal()));
        _binanceMetrics
            .CreatedOrdersCounter.Inc();
        return newOrderResponse;
    }
}