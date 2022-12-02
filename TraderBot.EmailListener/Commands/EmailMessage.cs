namespace TraderBot.EmailListener.Commands;

public record EmailMessage(string Id, string Subject, string HtmlBody, string TextBody)
{
}