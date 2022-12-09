namespace TraderBot.OrderController.Infrastructure;

internal class ExchangeInfoModel
{
    public IEnumerable<ExchangeSymbolInfoModel> Symbols { get; set; } = Array.Empty<ExchangeSymbolInfoModel>();
    
    internal class ExchangeSymbolInfoModel
    {
        public string Symbol { get; set; } = "";

        public IEnumerable<ExchangeSymbolFiltersInfoModel> Filters { get; set; } =
            Array.Empty<ExchangeSymbolFiltersInfoModel>();
    }

    internal class ExchangeSymbolFiltersInfoModel
    {
        public string FilterType { get; set; } = "";
        public string StepSize { get; set; } = "";
        public string MinQty { get; set; } = "";
        public string MaxQty { get; set; } = "";
    }
}