namespace TraderBot.Abstractions;

public class OrderSide
{
    public OrderSide(string value)
    {
        if (!Validate(value)) throw new ArgumentException($"OrderSide is no valid, value: '{value}'");
        Value = value;
    }

    public string Value { get; }

    private static bool Validate(string value)
    {
        return value == "SELL" || value == "BUY";
    }

    public static bool TryParse(string value, out OrderSide? orderSide)
    {
        var isValid = Validate(value);
        orderSide = isValid ? new OrderSide(value) : null;
        return isValid;
    }
}