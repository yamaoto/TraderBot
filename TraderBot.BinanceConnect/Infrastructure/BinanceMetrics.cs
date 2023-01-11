using System.Text.RegularExpressions;
using Prometheus;
using TraderBot.Abstractions;

namespace TraderBot.BinanceConnect.Infrastructure;

public class BinanceMetrics
{
    private readonly Dictionary<string, Histogram> _symbolHistogram = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<string, Gauge> _usdtBalance = new(StringComparer.InvariantCultureIgnoreCase);

    public Counter CreatedOrdersCounter { get; } =
        Metrics.CreateCounter("binance_order_total", "Total created Binance orders");

    public Histogram GetHistogramMetricForSymbol(string symbol, string histogramName, OrderSide side)
    {
        var name = $"binance_order_{symbol}_{side}_quantity";
        if (!_symbolHistogram.ContainsKey(name))
        {
            _symbolHistogram[name] = histogramName switch
            {
                "quantity" => Metrics.CreateHistogram(name, $"{symbol} {side} quantity"),
                "price" => Metrics.CreateHistogram(name, $"{symbol} {side} quantity"),
                _ => throw new ArgumentOutOfRangeException(nameof(histogramName), "Unsupported histogramName")
            };
        }

        return _symbolHistogram[name];
    }

    public Gauge GetUsdtBalance(string mailbox)
    {
        var name = $"binance_usdt_balance_{RemoveSpecialCharacters(mailbox)}";
        if (!_usdtBalance.ContainsKey(name))
        {
            _usdtBalance[name] = Metrics.CreateGauge(name, $"Binanc USDT balance {mailbox}");
        }

        return _usdtBalance[name];
    }

    private static string RemoveSpecialCharacters(string input) =>
        new Regex("(?:[^a-z0-9 ]|(?<=['\"])s)",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled)
            .Replace(input, String.Empty);
}