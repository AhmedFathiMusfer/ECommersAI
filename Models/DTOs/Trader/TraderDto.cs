using System;

namespace ECommersAI.DTOs.Trader
{
    public class TraderDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string WhatsAppId { get; set; } = string.Empty;
        public string SubscriptionStatus { get; set; } = string.Empty;
        public string DefaultCurrency { get; set; } = "USD";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
