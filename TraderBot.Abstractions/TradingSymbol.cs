namespace TraderBot.Abstractions;

public class TradingSymbol
{
    public TradingSymbol(string value)
    {
        if (!Validate(value)) throw new ArgumentException($"OrderType is no valid, value: '{value}'");
        Value = value;
    }

    public string Value { get; }

    private static bool Validate(string value)
    {
        return !string.IsNullOrEmpty(value);
    }

    public static bool TryParse(string value, out TradingSymbol? orderType)
    {
        var isValid = Validate(value);
        orderType = isValid ? new TradingSymbol(value) : null;
        return isValid;
    }
}