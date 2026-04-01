using System;

namespace ECommersAI.DTOs.Order
{
    public class AutoGenerateOrderRequest
    {
        public Guid TraderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string Currency { get; set; } = "USD";
        public string ConversationText { get; set; } = string.Empty;
    }
}
