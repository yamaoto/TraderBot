using TraderBot.Abstractions;

namespace TraderBot.EmailListener.Infrastructure;

public class MailBoxStore
{
    private readonly Dictionary<string, MailBoxSettings> _store = new Dictionary<string, MailBoxSettings>();

    public void Set(MailBoxSettings mailBoxSettings)
    {
        if (mailBoxSettings.MailBoxName == null)
        {
            throw new ArgumentException("MailBoxName can not be null", "MailBoxName");
        }

        _store[mailBoxSettings.MailBoxName] = mailBoxSettings;
    }

    public IEnumerable<MailBoxSettings> GetAll() => _store.Values;

    public MailBoxSettings GetByMailBoxName(string mailBoxName) => _store[mailBoxName];
}