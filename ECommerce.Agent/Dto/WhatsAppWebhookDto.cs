

namespace ECommerce.Agent.Dto
{
    public class WhatsAppWebhookDto
    {
        public string? Event { get; set; }
        public string? SessionId { get; set; }
        public WhatsAppData? Data { get; set; }
        public long Timestamp { get; set; }
    }

    public class WhatsAppData
    {
        public WhatsAppMessageWrapper? Messages { get; set; }
    }

    public class WhatsAppMessageWrapper
    {
        public WhatsAppKey? Key { get; set; }
        public long MessageTimestamp { get; set; }
        public string? PushName { get; set; }
        public bool Broadcast { get; set; }
        public WhatsAppMessage? Message { get; set; }
        public string? MessageBody { get; set; }
        public string? RemoteJid { get; set; }
        public string? Id { get; set; }
    }

    public class WhatsAppKey
    {
        public string? Id { get; set; }
        public bool FromMe { get; set; }
        public string? RemoteJid { get; set; }
        public string? SenderPn { get; set; }
        public string? CleanedSenderPn { get; set; }
        public string? SenderLid { get; set; }
        public string? AddressingMode { get; set; }
    }

    public class WhatsAppMessage
    {
        public ExtendedTextMessage? ExtendedTextMessage { get; set; }
    }

    public class ExtendedTextMessage
    {
        public string? Text { get; set; }
    }
}