namespace Trading.Domain.Entities;

using Trading.Domain.SeedWork;

public class Trade(string userId, string asset, int quantity, decimal price)
    : EntityBase, IAggregateRoot
{
    public string UserId { get; set; } = userId;
    public string Asset { get; set; } = asset;
    public int Quantity { get; set; } = quantity;
    public decimal Price { get; set; } = price;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}