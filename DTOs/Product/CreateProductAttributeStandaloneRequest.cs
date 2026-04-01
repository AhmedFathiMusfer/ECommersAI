using System;

namespace ECommersAI.DTOs.Product
{
    public class CreateProductAttributeStandaloneRequest
    {
        public Guid ProductId { get; set; }
        public string AttributeName { get; set; } = string.Empty;
        public string AttributeValue { get; set; } = string.Empty;
    }
}
