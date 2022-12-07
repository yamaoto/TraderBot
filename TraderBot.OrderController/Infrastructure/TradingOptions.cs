namespace TraderBot.OrderController.Infrastructure;

public class TradingOptions : Dictionary<string, TradingOptions.TradingSymbolOptions>
{
    public TradingOptions() : base(StringComparer.InvariantCultureIgnoreCase)
    {
    }

    public TradingSymbolOptions GetOptionsForSymbol(string symbol)
    {
        return ContainsKey(symbol) ? this[symbol] : this["default"];
    }

    public class TradingSymbolOptions
    {
        public decimal Rate { get; set; }
    }
}