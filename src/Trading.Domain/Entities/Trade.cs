namespace Trading.Domain.Entities;

using Trading.Domain.SeedWork;

public class Trade : EntityBase, IAggregateRoot
{
    public string UserId { get; set; }
    public string Asset { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime Timestamp { get; set; }

    public Trade(string userId, string asset, int quantity, decimal price)
    {
        UserId = userId;
        Asset = asset;
        Quantity = quantity;
        Price = price;
        Timestamp = DateTime.UtcNow;
    }
}