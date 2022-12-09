using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Options;
using TraderBot.EmailListener.Commands;
using TraderBot.EmailListener.Infrastructure;
using TraderBot.RavenDb.MailBoxDomain;

namespace TraderBot.EmailListener.HostedServices;

public class EmailListenerStateMachine : IDisposable
{
    private readonly MailBoxSettingRecord _settings;
    private readonly ILogger<EmailListenerStateMachine> _logger;
    private readonly EmailChannel _emailChannel;
    private readonly IOptions<MailBoxOptions> _mailBoxOptions;
    private readonly Dictionary<string, DateTime> _processedMessages = new Dictionary<string, DateTime>();

    private DateTime? _waitNextTime;
    private ImapClient? _imapClient;
    private DateTime? _latestCollectedAt;

    public EmailListenerStateMachine(
        EmailChannel emailChannel,
        IOptions<MailBoxOptions> mailBoxOptions,
        MailBoxSettingRecord settings,
        EmailListenerStatus state,
        ILogger<EmailListenerStateMachine> logger
        )
    {
        _emailChannel = emailChannel;
        _mailBoxOptions = mailBoxOptions;
        _settings = settings;
        _logger = logger;
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
            _logger.LogError("Mailbox {MailBox} error. See original error: {ErrorDetails}", _settings.Name, e);
        }
    }

    private async Task ConnectAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Connecting to mailbox {MailBox}", _settings.Name);
        _imapClient = new ImapClient(new NullProtocolLogger());
        await _imapClient.ConnectAsync(_mailBoxOptions.Value.Host, _mailBoxOptions.Value.Port,
            SecureSocketOptions.SslOnConnect, cancellationToken);
        await _imapClient.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
        await _imapClient.Inbox.OpenAsync(FolderAccess.ReadOnly, cancellationToken);
        _logger.LogDebug("Successfully connected to {MailBox}", _settings.Name);
    }

    private async Task<IEnumerable<EmailMessage>> CollectEmailsAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Collecting new messages for {MailBox}", _settings.Name);
        _latestCollectedAt ??= DateTime.UtcNow.AddHours(-1);

        var query = SearchQuery.And(SearchQuery.DeliveredAfter(_latestCollectedAt.Value),
            SearchQuery.FromContains($"<{_mailBoxOptions.Value.GetFrom}>"));
        var latestMessageIds = await _imapClient!.Inbox.SearchAsync(query, cancellationToken);
        _latestCollectedAt = DateTime.UtcNow;
        var response = new List<EmailMessage>();
        foreach (var uid in latestMessageIds)
        {
            var uidString = uid.ToString();
            if (_processedMessages.ContainsKey(uidString)) break;
            var message = await _imapClient.Inbox.GetMessageAsync(uid, cancellationToken);
            response.Add(new EmailMessage(_settings.Name, uid.ToString(), message.Subject, message.HtmlBody));
            _processedMessages[uidString] = DateTime.Now;
            _logger.LogDebug("Add to queue {MailBox} message with uid {Uid}", _settings.Name, uidString);
        }

        CleanProcessedMessages();
        return response;
    }

    private void CleanProcessedMessages()
    {
        var expired = DateTime.Now.AddMinutes(-10);
        foreach (var (key, timestamp) in _processedMessages.ToList())
        {
            if (expired > timestamp)
            {
                _processedMessages.Remove(key);
            }
        }
    }
    
    public void Dispose()
    {
        _imapClient?.Dispose();
    }
}