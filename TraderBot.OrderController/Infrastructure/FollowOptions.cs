namespace TraderBot.OrderController.Infrastructure;

public class FollowOptions
{
    public IEnumerable<string> Allowed { get; set; } = Array.Empty<string>();
}