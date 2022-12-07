using Microsoft.Extensions.Options;
using TraderBot.EmailListener.Infrastructure;

namespace TraderBot.EmailListener.HostedServices;

public class EmailListenerJob : BackgroundService
{
    private readonly MailBoxStore _mailBoxStore;
    private readonly EmailChannel _emailChannel;
    private readonly IOptions<MailBoxOptions> _mailBoxOptions;
    private readonly Dictionary<string, EmailListenerStateMachine> Listeners;


    public EmailListenerJob(MailBoxStore mailBoxStore, EmailChannel emailChannel,
        IOptions<MailBoxOptions> mailBoxOptions)
    {
        _mailBoxStore = mailBoxStore;
        _emailChannel = emailChannel;
        _mailBoxOptions = mailBoxOptions;
        Listeners = new Dictionary<string, EmailListenerStateMachine>();
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var mailBox in _mailBoxStore.GetAll())
            {
                if (!Listeners.ContainsKey(mailBox.MailBoxName))
                {
                    Listeners[mailBox.MailBoxName] = new EmailListenerStateMachine(_emailChannel, _mailBoxOptions,
                        mailBox, EmailListenerStatus.New);
                }

                if (Listeners[mailBox.MailBoxName].State == EmailListenerStatus.Error)
                {
                    continue;
                }
                await Listeners[mailBox.MailBoxName].ExecuteNextAction(cancellationToken);
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