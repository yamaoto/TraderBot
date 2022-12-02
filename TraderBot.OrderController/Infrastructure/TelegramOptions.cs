namespace TraderBot.OrderController.Infrastructure;

public class TelegramOptions
{
    public string Url { get; set; } = "";
    public bool Enabled { get; set; }

    public string DefaultChatId { get; set; } = "";
}