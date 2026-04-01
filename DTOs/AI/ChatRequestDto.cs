using System;

namespace ECommersAI.DTOs.AI
{
    public class ChatRequestDto
    {
        public Guid TraderId { get; set; }
        public string CustomerPhone { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
