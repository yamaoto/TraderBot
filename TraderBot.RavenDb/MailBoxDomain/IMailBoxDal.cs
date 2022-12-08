namespace TraderBot.RavenDb.MailBoxDomain;

public interface IMailBoxDal
{
    Task<MailBoxSettingRecord> GetMailBoxAsync(string name, CancellationToken cancellationToken = default);

    Task<IEnumerable<MailBoxSettingRecord>> GetMailBoxesAsync(CancellationToken cancellationToken = default);

    Task UpsertMailBoxAsync(MailBoxSettingRecord mailBox, CancellationToken cancellationToken = default);
    Task DeleteMailBoxAsync(string name, CancellationToken cancellationToken = default);
}