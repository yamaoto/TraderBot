namespace TraderBot.RavenDb.MailBoxDomain;

public record MailBoxSettingRecord(
    string Name,
    string Username,
    string Password,
    string BinanceApiKey,
    string BinanceApiSecret
);