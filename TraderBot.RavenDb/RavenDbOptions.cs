namespace TraderBot.RavenDb;

public class RavenDbOptions
{
    public IEnumerable<string> Urls { get; set; } = Array.Empty<string>();
    public string DatabaseName { get; set; } = "TraderBot";
}