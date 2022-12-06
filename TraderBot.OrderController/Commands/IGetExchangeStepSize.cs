namespace TraderBot.OrderController.Commands;

public interface IGetExchangeStepSize
{
    Task<decimal> GetExchangeStepSizeAsync(string symbol);
}