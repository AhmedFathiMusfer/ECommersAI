using System;
using System.Collections.Generic;

namespace ECommersAI.Models.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid TraderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public string Currency { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Trader Trader { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}