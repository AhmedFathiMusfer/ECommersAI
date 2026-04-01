using System.Collections.Generic;

namespace ECommersAI.DTOs.Product
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PriceUSD { get; set; }
        public string Category { get; set; } = string.Empty;
        public List<CreateProductImageRequest> Images { get; set; } = new();
        public List<CreateProductAttributeRequest> Attributes { get; set; } = new();
    }

    public class CreateProductImageRequest
    {
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
    }

    public class CreateProductAttributeRequest
    {
        public string AttributeName { get; set; } = string.Empty;
        public string AttributeValue { get; set; } = string.Empty;
    }
}
