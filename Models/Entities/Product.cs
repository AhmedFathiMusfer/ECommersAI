using System;
using System.Collections.Generic;

namespace ECommersAI.Models.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid TraderId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal PriceUSD { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Trader Trader { get; set; }
        public ICollection<ProductImage> Images { get; set; }
        public ICollection<ProductAttribute> Attributes { get; set; }
        public ProductVector ProductVector { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}