namespace TraderBot.BinanceConnect.BinanceSupport;

public record FuturesAccountBalanceItem
{
    public string AccountAlias { get; init; } = "";
    public string Asset { get; init; } = "";
    public string Balance { get; init; } = "";
    public string CrossWalletBalance { get; init; } = "";
    public string CrossUnPnl { get; init; } = "";
    public string AvailableBalance { get; init; } = "";
    public string MaxWithdrawAmount { get; init; } = "";
    public bool MarginAvailable { get; init; }
    public long UpdateTime { get; init; }
}