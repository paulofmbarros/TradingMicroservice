﻿namespace Trading.Application.DTO;

public class TradeDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string Asset { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime Timestamp { get; set; }
}