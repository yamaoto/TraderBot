namespace TraderBot.RavenDb;

public class RavenDbOptions
{
    public string Endpoint { get; set; } = "";
    public string DatabaseName { get; set; } = "TraderBot";
    public bool UseStub { get; set; }
}