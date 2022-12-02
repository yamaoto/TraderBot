namespace TraderBot.BinanceConnect.Infrastructure;

public class BinanceOptions
{
    public string ApiKey { get; set; } = "";
    public string SecretKey { get; set; } = "";
    public bool UseTestnet { get; set; }
}