using System;

namespace ECommersAI.DTOs.Product
{
    public class ProductSearchResultDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PriceUSD { get; set; }
        public decimal DisplayPrice { get; set; }
        public string Currency { get; set; } = "USD";
        public string Category { get; set; } = string.Empty;
        public double SimilarityScore { get; set; }
    }
}
