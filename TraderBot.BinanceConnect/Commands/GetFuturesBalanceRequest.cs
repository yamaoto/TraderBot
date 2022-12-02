using System.Globalization;
using Binance.Common;
using Microsoft.Extensions.Options;
using TraderBot.BinanceConnect.BinanceSupport;
using TraderBot.BinanceConnect.Infrastructure;

namespace TraderBot.BinanceConnect.Commands;

public class GetFuturesBalanceRequest
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<BinanceOptions> _options;

    public GetFuturesBalanceRequest(
        HttpClient httpClient,
        IOptions<BinanceOptions> options
    )
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<decimal> GetFuturesBalanceAsync()
    {
        var baseUrl = _options.Value.UseTestnet ? "https://testnet.binancefuture.com" : "https://fapi.binance.com";
        var futureTrade = new FuturesTradeClient(_httpClient, baseUrl, _options.Value.ApiKey,
            new BinanceHmac(_options.Value.SecretKey)
        );
        var balance = await futureTrade.GetFuturesAccountBalanceV2Async();
        var usdtBalance = balance.FirstOrDefault(f => f.Asset == "USDT");
        if (usdtBalance == null) throw new Exception("There is not found USDT wallet in futures account");

        if (!decimal.TryParse(usdtBalance.AvailableBalance, CultureInfo.InvariantCulture, out var usdtBalanceAmount))
            throw new InvalidOperationException("Failed AvailableBalance parsing");

        return usdtBalanceAmount;
    }
}