namespace TraderBot.BinanceConnect.BinanceSupport;

public class FuturesNewOrderResponse
{
    public ulong OrderId { get; set; }
    public string Symbol { get; set; } = "";
    public string Status { get; set; } = "";
    public string ClientOrderId { get; set; } = "";
    public string Price { get; set; } = "";
    public string AvgPrice { get; set; } = "";
    public string OrigQty { get; set; } = "";
    public string ExecutedQty { get; set; } = "";

    public string CumQty { get; set; } = "";
    public string CumQuote { get; set; } = "";
    public string TimeInForce { get; set; } = "";
    public string Type { get; set; } = "";

    public bool ReduceOnly { get; set; }
    public bool ClosePosition { get; set; }

    public string Side { get; set; } = "";
    public string PositionSide { get; set; } = "";
    public string StopPrice { get; set; } = "";
    public string WorkingType { get; set; } = "";
    public bool PriceProtect { get; set; }

    public string OrigType { get; set; } = "";
    public long UpdateTime { get; set; }

    public string? ActivatePrice { get; set; }
    public string? PriceRate { get; set; }
}