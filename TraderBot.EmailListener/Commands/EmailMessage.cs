namespace TraderBot.EmailListener.Commands;

public record EmailMessage(string Mailbox, string Id, string Subject, string HtmlBody)
{
}