using System;
using System.Collections.Generic;

namespace ECommersAI.DTOs.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public Guid TraderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PriceUSD { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();
        public List<ProductAttributeDto> Attributes { get; set; } = new();
    }
}
