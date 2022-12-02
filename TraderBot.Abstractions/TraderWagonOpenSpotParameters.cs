namespace TraderBot.Abstractions;

public record TraderWagonOpenSpotParameters
    (
        TradingSymbol TradingSymbol,
        OrderSide OpeningSide,
        decimal Price
    )
    : RecordWithValidation
{
    public string? From { get; init; }
    public DateTime? At { get; init; }

    protected override void Validate()
    {
        if (Price <= 0) throw new ArgumentException("Quantity must be positive");
    }
}