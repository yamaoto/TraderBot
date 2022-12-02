namespace TraderBot.Abstractions;

public record TraderWagonCloseSpotParameters
(
    string From,
    DateTime At,
    TradingSymbol TradingSymbol,
    decimal ClosePrice,
    decimal RealizedProfit
)
{
}