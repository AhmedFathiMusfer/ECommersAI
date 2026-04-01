using System;
using System.Collections.Generic;

namespace ECommersAI.DTOs.Order
{
    public class CreateOrderRequest
    {
        public Guid TraderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string Currency { get; set; } = "USD";
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }

    public class CreateOrderItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
