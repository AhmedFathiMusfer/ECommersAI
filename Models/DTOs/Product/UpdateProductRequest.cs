using System.Collections.Generic;

namespace ECommersAI.DTOs.Product
{
    public class UpdateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PriceUSD { get; set; }
        public string Category { get; set; } = string.Empty;
        public List<CreateProductImageRequest> Images { get; set; } = new();
        public List<CreateProductAttributeRequest> Attributes { get; set; } = new();
    }
}
