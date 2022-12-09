using System.Globalization;
using TraderBot.Abstractions;
using TraderBot.OrderController.Infrastructure;

namespace TraderBot.OrderController.Commands;

public class GetExchangeStepSize : IGetExchangeStepSize
{
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, ExchangeSymbolInfo> _exchangeStepSize = new (StringComparer.InvariantCultureIgnoreCase);

    public GetExchangeStepSize(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ExchangeSymbolInfo> GetExchangeSymbolInfoAsync(string symbol)
    {
        // TODO: To ValueTask
        if (_exchangeStepSize.Count == 0)
        {
            var exchangeInfo =
                await _httpClient.GetFromJsonAsync<ExchangeInfoModel>("https://fapi.binance.com/fapi/v1/exchangeInfo");
            foreach (var exchangeSymbol in exchangeInfo!.Symbols)
            {
                var lotSize = exchangeSymbol.Filters.FirstOrDefault(f => f.FilterType == "LOT_SIZE");
                if (lotSize != null 
                    && decimal.TryParse(lotSize.StepSize, CultureInfo.InvariantCulture, out var stepSize)
                    && decimal.TryParse(lotSize.MinQty, CultureInfo.InvariantCulture, out var min)
                    && decimal.TryParse(lotSize.MaxQty, CultureInfo.InvariantCulture, out var max))
                {
                    _exchangeStepSize[exchangeSymbol.Symbol] = new ExchangeSymbolInfo(stepSize, min, max);
                }
            }
        }

        if (_exchangeStepSize.ContainsKey(symbol))
        {
            return _exchangeStepSize[symbol];
        }

        throw new AppException("UNKNOWN_EXCHANGE_SYMBOL", $"Trading symbol {symbol} not found in exchange info");
    }
}