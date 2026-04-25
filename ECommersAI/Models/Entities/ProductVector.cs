using System;
using Pgvector;

namespace ECommersAI.Models.Entities
{
    public class ProductVector
    {
        public Guid ProductId { get; set; }
        public Vector Vector { get; set; } // 1536-dim vector

        public Product Product { get; set; }
    }
}