using System;

namespace ECommersAI.DTOs.Product
{
    public class CreateProductImageStandaloneRequest
    {
        public Guid ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
    }
}
