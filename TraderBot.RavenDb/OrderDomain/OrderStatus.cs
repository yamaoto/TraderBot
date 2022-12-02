namespace TraderBot.RavenDb.OrderDomain;

[Flags]
public enum OrderStatus
{
    No = 0,
    Created = 1,
    Closed = 2
}