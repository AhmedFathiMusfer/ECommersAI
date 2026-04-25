using System;
using System.Collections.Generic;

namespace ECommersAI.Models.Entities
{
    public class Trader
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string WhatsAppId { get; set; }
        public string SubscriptionStatus { get; set; }
        public string DefaultCurrency { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Product> Products { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}