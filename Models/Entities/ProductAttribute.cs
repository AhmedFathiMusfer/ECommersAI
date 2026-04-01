using System;

namespace ECommersAI.Models.Entities
{
    public class ProductAttribute
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }

        public Product Product { get; set; }
    }
}