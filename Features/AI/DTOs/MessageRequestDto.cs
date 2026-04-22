namespace ECommersAI.Features.AI.DTOs
{
    public class MessageRequestDto
    {
        public Guid TraderId { get; set; }
        public string CustomerPhone { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string MessageType { get; set; } = string.Empty;
    }
}
