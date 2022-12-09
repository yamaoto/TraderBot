namespace TraderBot.RavenDb.MailBoxDomain;

public class StubMailBoxDal : IMailBoxDal
{
    private readonly Dictionary<string, MailBoxSettingRecord> _mailboxes = new Dictionary<string, MailBoxSettingRecord>();
    public Task<MailBoxSettingRecord> GetMailBoxAsync(string name, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_mailboxes[name]);
    }

    public Task<IEnumerable<MailBoxSettingRecord>> GetMailBoxesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult((IEnumerable<MailBoxSettingRecord>)_mailboxes);
    }

    public Task UpsertMailBoxAsync(MailBoxSettingRecord mailBox, CancellationToken cancellationToken = default)
    {
        _mailboxes[mailBox.Name] = mailBox;
        return Task.CompletedTask;
    }

    public Task DeleteMailBoxAsync(string name, CancellationToken cancellationToken = default)
    {
        _mailboxes.Remove(name);
        return Task.CompletedTask;
    }
}