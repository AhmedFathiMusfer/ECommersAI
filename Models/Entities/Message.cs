using System;

namespace ECommersAI.Models.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid TraderId { get; set; }
        public string CustomerPhone { get; set; }
        public string MessageType { get; set; }
        public string Content { get; set; }
        public string AIResponse { get; set; }
        public DateTime CreatedAt { get; set; }

        public Trader Trader { get; set; }
    }
}