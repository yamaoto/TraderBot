namespace TraderBot.RavenDb.OrderDomain;

public record OrderRecord
{
    public required string Id { get; init; }
    public required string Mailbox { get; init; }
    public required string Symbol { get; set; }
    public required string OrderType { get; set; }
    public required string CopyFrom { get; set; }
    public required decimal Quantity { get; set; }
    public required decimal Price { get; set; }
    public required DateTime OrderCreatedAt { get; set; }
    public required OrderStatus Status { get; set; }
}