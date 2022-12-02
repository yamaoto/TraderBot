namespace TraderBot.EmailListener.Infrastructure;

public class MailBoxOptions
{
    public string MailBoxName { get; set; } = "";
    public string Host { get; set; } = "";
    public int Port { get; set; } = 993;
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";

    public string GetFrom { get; set; } = "do-not-reply@post.traderwagon.com";
}