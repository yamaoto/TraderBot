namespace TraderBot.OrderController.Commands;

public interface IGetExchangeStepSize
{
    Task<ExchangeSymbolInfo> GetExchangeSymbolInfoAsync(string symbol);
}