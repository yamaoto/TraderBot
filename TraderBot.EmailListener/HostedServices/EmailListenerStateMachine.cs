using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Options;
using TraderBot.EmailListener.Commands;
using TraderBot.EmailListener.Infrastructure;

namespace TraderBot.EmailListener.HostedServices;

public class EmailListenerStateMachine : IDisposable
{
    private readonly MailBoxSettings _settings;
    private readonly EmailChannel _emailChannel;
    private readonly IOptions<MailBoxOptions> _mailBoxOptions;

    private DateTime? _waitNextTime;
    private ImapClient? _imapClient;
    private DateTime? _latestCollectedAt;
    private string? _lastProcessedEmail;

    public EmailListenerStateMachine(EmailChannel emailChannel, IOptions<MailBoxOptions> mailBoxOptions,
        MailBoxSettings settings, EmailListenerStatus state)
    {
        _emailChannel = emailChannel;
        _mailBoxOptions = mailBoxOptions;
        _settings = settings;
        State = state;
    }

    public EmailListenerStatus State { get; set; }
    public Exception? Error { get; set; }

    public async Task ExecuteNextAction(CancellationToken cancellationToken)
    {
        try
        {
            switch (State)
            {
                case EmailListenerStatus.New:
                    await ConnectAsync(cancellationToken);
                    State = EmailListenerStatus.Connected;
                    break;
                case EmailListenerStatus.Connected:
                    var emails = await CollectEmailsAsync(cancellationToken);
                    foreach (var email in emails)
                    {
                        await _emailChannel.ChannelWriter.WriteAsync(email, cancellationToken);
                    }

                    State = EmailListenerStatus.Wait;
                    _waitNextTime = DateTime.Now.AddSeconds(5);
                    break;
                case EmailListenerStatus.Wait:
                    if (_waitNextTime == null || DateTime.Now >= _waitNextTime)
                    {
                        State = EmailListenerStatus.Connected;
                    }

                    break;
                case EmailListenerStatus.Error:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        catch (Exception e)
        {
            State = EmailListenerStatus.Error;
            Error = e;
        }
    }

    private async Task ConnectAsync(CancellationToken cancellationToken)
    {
        _imapClient = new ImapClient(new NullProtocolLogger());
        await _imapClient.ConnectAsync(_mailBoxOptions.Value.Host, _mailBoxOptions.Value.Port,
            SecureSocketOptions.SslOnConnect, cancellationToken);
        await _imapClient.AuthenticateAsync(_settings.Username, _settings.AppPassword, cancellationToken);
        await _imapClient.Inbox.OpenAsync(FolderAccess.ReadOnly, cancellationToken);
    }

    private async Task<IEnumerable<EmailMessage>> CollectEmailsAsync(CancellationToken cancellationToken)
    {
        _latestCollectedAt ??= DateTime.Today.AddHours(-1);

        var query = SearchQuery.And(SearchQuery.DeliveredAfter(_latestCollectedAt.Value),
            SearchQuery.FromContains($"<{_mailBoxOptions.Value.GetFrom}>"));
        var latestMessageIds = await _imapClient!.Inbox.SearchAsync(query, cancellationToken);
        _latestCollectedAt = DateTime.Now;
        var response = new List<EmailMessage>();
        foreach (var uid in latestMessageIds)
        {
            if (uid.ToString() == _lastProcessedEmail) break;
            var message = await _imapClient.Inbox.GetMessageAsync(uid, cancellationToken);
            response.Add(new EmailMessage(uid.ToString(), message.Subject, message.HtmlBody, message.TextBody));
            _lastProcessedEmail = uid.ToString();
        }

        return response;
    }

    public void Dispose()
    {
        _imapClient?.Dispose();
    }
}