using System;
using System.Collections.Generic;

namespace ECommersAI.DTOs.Order
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid TraderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string Currency { get; set; } = "USD";
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPriceUSD { get; set; }
        public decimal ConvertedPrice { get; set; }
    }
}
