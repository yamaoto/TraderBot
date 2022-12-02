namespace TraderBot.RavenDb;

public class RavenDbOptions
{
    public IEnumerable<string> Urls { get; set; } = new[] { "http://127.0.0.1:8080" };
    public string DatabaseName { get; set; } = "TraderBor";
}