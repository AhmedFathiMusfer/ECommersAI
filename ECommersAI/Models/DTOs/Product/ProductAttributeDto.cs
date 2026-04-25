using System;

namespace ECommersAI.DTOs.Product
{
    public class ProductAttributeDto
    {
        public Guid Id { get; set; }
        public string AttributeName { get; set; } = string.Empty;
        public string AttributeValue { get; set; } = string.Empty;
    }
}
