using System;

namespace ECommersAI.DTOs.Message
{
    public class WhatsAppWebhookRequest
    {
        public Guid TraderId { get; set; }
        public string CustomerPhone { get; set; } = string.Empty;
        public string MessageType { get; set; } = "text";
        public string Content { get; set; } = string.Empty;
        public string? MediaUrl { get; set; }
    }
}
