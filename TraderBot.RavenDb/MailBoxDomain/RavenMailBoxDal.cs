using Raven.Client.Documents;

namespace TraderBot.RavenDb.MailBoxDomain;

public class RavenMailBoxDal : IMailBoxDal
{
    private readonly IDocumentStore _documentStore;

    public RavenMailBoxDal(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<MailBoxSettingRecord> GetMailBoxAsync(string name, CancellationToken cancellationToken = default)
    {
        using var session = _documentStore.OpenAsyncSession();
        var mailBox = await session.LoadAsync<MailBoxSettingRecord>(name, cancellationToken);
        return mailBox;
    }

    public async Task<IEnumerable<MailBoxSettingRecord>> GetMailBoxesAsync(
        CancellationToken cancellationToken = default)
    {
        using var session = _documentStore.OpenAsyncSession();
        var mailBoxes = await session.Query<MailBoxSettingRecord>()
            .OrderBy(o => o.Name)
            .ToListAsync(cancellationToken);
        return mailBoxes;
    }

    public async Task UpsertMailBoxAsync(MailBoxSettingRecord mailBox, CancellationToken cancellationToken = default)
    {
        using var session = _documentStore.OpenAsyncSession();
        await session.StoreAsync(mailBox, mailBox.Name, cancellationToken);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteMailBoxAsync(string name, CancellationToken cancellationToken = default)
    {
        using var session = _documentStore.OpenAsyncSession();
        session.Delete(name);
        await session.SaveChangesAsync(cancellationToken);
    }
}