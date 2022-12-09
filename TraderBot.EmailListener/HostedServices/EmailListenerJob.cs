using Microsoft.Extensions.Options;
using TraderBot.EmailListener.Infrastructure;
using TraderBot.RavenDb.MailBoxDomain;

namespace TraderBot.EmailListener.HostedServices;

public class EmailListenerJob : BackgroundService
{
    private readonly IMailBoxDal _mailBoxDal;
    private readonly EmailChannel _emailChannel;
    private readonly IOptions<MailBoxOptions> _mailBoxOptions;
    private readonly ILoggerFactory _loggerFactory;
    private readonly Dictionary<string, EmailListenerStateMachine> Listeners;


    public EmailListenerJob(
        IMailBoxDal mailBoxDal,
        EmailChannel emailChannel,
        IOptions<MailBoxOptions> mailBoxOptions,
        ILoggerFactory loggerFactory
        )
    {
        _mailBoxDal = mailBoxDal;
        _emailChannel = emailChannel;
        _mailBoxOptions = mailBoxOptions;
        _loggerFactory = loggerFactory;
        Listeners = new Dictionary<string, EmailListenerStateMachine>();
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var logger = _loggerFactory.CreateLogger<EmailListenerJob>();
        logger.LogInformation("Starting EmailListenerJob background service");
        while (!cancellationToken.IsCancellationRequested)
        {
            var mailBoxes = await _mailBoxDal.GetMailBoxesAsync(cancellationToken);
            foreach (var mailBox in mailBoxes)
            {
                if (!Listeners.ContainsKey(mailBox.Name))
                {
                    Listeners[mailBox.Name] = new EmailListenerStateMachine(_emailChannel, _mailBoxOptions,
                        mailBox, EmailListenerStatus.New, _loggerFactory.CreateLogger<EmailListenerStateMachine>());
                }

                if (Listeners[mailBox.Name].State == EmailListenerStatus.Error)
                {
                    continue;
                }
                await Listeners[mailBox.Name].ExecuteNextAction(cancellationToken);
            }

            await Task.Delay(100, cancellationToken);
        }
    }

    public override void Dispose()
    {
        foreach (var (name, stateMachine) in Listeners)
        {
            stateMachine.Dispose();
        }

        base.Dispose();
    }
}