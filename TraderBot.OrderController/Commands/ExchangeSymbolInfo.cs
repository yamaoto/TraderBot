namespace TraderBot.OrderController.Commands;

public record ExchangeSymbolInfo (
    decimal StepSize,
    decimal MinimalQuantity,
    decimal MaximalQuantity
)
{
    
}