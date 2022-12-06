using System.Globalization;
using Binance.Common;
using Newtonsoft.Json;
using TraderBot.BinanceConnect.Infrastructure;
using TraderBot.BinanceConnectProto;
using TraderBot.TypesProto;

namespace TraderBot.BinanceConnect.BinanceSupport;

public class FuturesTradeClient : BinanceService
{
    public FuturesTradeClient(
        HttpClient httpClient,
        string baseUrl, string apiKey, string apiSecret) : base(httpClient,
        baseUrl, apiKey, apiSecret)
    {
    }

    public FuturesTradeClient(HttpClient httpClient, string baseUrl, string apiKey,
        IBinanceSignatureService signatureService) : base(httpClient, baseUrl, apiKey, signatureService)
    {
    }

    public async Task<FuturesNewOrderResponse> NewOrderAsync(OpenSpotRequest openSpotParameters)
    {
        var parameters = new Dictionary<string, object>
        {
            ["symbol"] = openSpotParameters.TradingSymbol,
            ["side"] = openSpotParameters.OrderSide == OrderSideType.Sell ? "SELL" : "BUY",
            ["type"] = "LIMIT",
            ["recvWindow"] = 5000,
            ["quantity"] = openSpotParameters.Quantity.ConvertToRegularDecimal().ToString(CultureInfo.InvariantCulture),
            ["price"] = openSpotParameters.Price.ConvertToRegularDecimal().ToString(CultureInfo.InvariantCulture),
            ["timeInForce"] = "GTC",
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        string json;
        try
        {
            json = await SendSignedAsync<string>("/fapi/v1/order", HttpMethod.Post, parameters);
        }
        catch (Exception)
        {
            // Debug breakpoint
            throw;
        }
        var newOrderResponse = JsonConvert.DeserializeObject<FuturesNewOrderResponse>(json);
        if (newOrderResponse!.Status != "NEW")
        {
            throw new Exception($"New open spot order failed with status '{newOrderResponse.Status}'")
            {
                Data = { ["OriginalData"] = json }
            };
        }
        return newOrderResponse;
    }

    public async Task<FuturesAccountBalanceResponse> GetFuturesAccountBalanceV2Async()
    {
        var parameters = new Dictionary<string, object>
        {
            ["recvWindow"] = 5000,
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        var json = await SendSignedAsync<string>("/fapi/v2/balance", HttpMethod.Get, parameters);
        var balanceResponse = JsonConvert.DeserializeObject<FuturesAccountBalanceResponse>(json);
        return balanceResponse!;
    }
}