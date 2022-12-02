namespace TraderBot.OrderController.Infrastructure;

internal class ExchangeInfoModel
{
    public IEnumerable<ExchangeSymbolInfoModel> Symbols { get; set; }
    
    internal class ExchangeSymbolInfoModel
    {
        public string Symbol { get; set; }
        public IEnumerable<ExchangeSymbolFiltersInfoModel> Filters { get; set; }
    }

    internal class ExchangeSymbolFiltersInfoModel
    {
        public string FilterType { get; set; }
        public string StepSize { get; set; }
    }
}