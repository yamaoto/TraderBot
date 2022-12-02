using Binance.Common;
using Microsoft.Extensions.Options;
using TraderBot.BinanceConnect.BinanceSupport;
using TraderBot.BinanceConnect.Infrastructure;
using TraderBot.BinanceConnectProto;

namespace TraderBot.BinanceConnect.Commands;

public class OpenSpotCommand
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<BinanceOptions> _options;

    public OpenSpotCommand(
        HttpClient httpClient,
        IOptions<BinanceOptions> options
    )
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<FuturesNewOrderResponse> OpenSpotAsync(OpenSpotRequest openSpotParameters)
    {
        var baseUrl = _options.Value.UseTestnet ? "https://testnet.binancefuture.com" : "https://fapi.binance.com";
        var futureTrade = new FuturesTradeClient(_httpClient, baseUrl, _options.Value.ApiKey,
            new BinanceHmac(_options.Value.SecretKey)
        );
        var newOrderResponse = await futureTrade.NewOrderAsync(openSpotParameters);
        return newOrderResponse;
    }
}