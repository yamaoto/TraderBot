namespace TraderBot.EmailListener.Infrastructure;

public class MailBoxOptions
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 993;

    public string GetFrom { get; set; } = "do-not-reply@post.traderwagon.com";
}