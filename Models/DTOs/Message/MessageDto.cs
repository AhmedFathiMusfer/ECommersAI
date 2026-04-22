using System;

namespace ECommersAI.DTOs.Message
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid TraderId { get; set; }
        public string CustomerPhone { get; set; } = string.Empty;
        public string MessageType { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string AIResponse { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
