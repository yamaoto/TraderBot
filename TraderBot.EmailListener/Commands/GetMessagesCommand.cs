using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Options;
using TraderBot.EmailListener.Infrastructure;

namespace TraderBot.EmailListener.Commands;

public class GetMessagesCommand
{
    private readonly IOptions<MailBoxOptions> _mailBoxOptions;

    public GetMessagesCommand(IOptions<MailBoxOptions> mailBoxOptions)
    {
        _mailBoxOptions = mailBoxOptions;
    }

    public async Task<IEnumerable<EmailMessage>> GetMessagesAsync(DateTime latest, string lastProcessedEmail)
    {
        if (_mailBoxOptions.Value == null) return Array.Empty<EmailMessage>();

        using var client = new ImapClient(new NullProtocolLogger());
        await client.ConnectAsync(_mailBoxOptions.Value.Host, _mailBoxOptions.Value.Port,
            SecureSocketOptions.SslOnConnect);
        await client.AuthenticateAsync(_mailBoxOptions.Value.Username, _mailBoxOptions.Value.Password);
        await client.Inbox.OpenAsync(FolderAccess.ReadOnly);
        var query = SearchQuery.And(SearchQuery.DeliveredAfter(latest),
            SearchQuery.FromContains($"<{_mailBoxOptions.Value.GetFrom}>"));
        var latestMessageIds = await client.Inbox.SearchAsync(query);
        var response = new List<EmailMessage>();
        foreach (var uid in latestMessageIds)
        {
            if (uid.ToString() == lastProcessedEmail) break;
            var message = await client.Inbox.GetMessageAsync(uid);
            response.Add(new EmailMessage(uid.ToString(), message.Subject, message.HtmlBody, message.TextBody));
        }

        await client.DisconnectAsync(true);
        return response;
    }
}