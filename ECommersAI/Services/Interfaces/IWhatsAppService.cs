
using ECommersAI.DTOs.Message;

namespace ECommersAI.Services.Interfaces
{
    public interface IWhatsAppService
    {
        Task QueueIncomingMessageAsync(WhatsAppWebhookRequest request);
        Task ProcessIncomingMessageAsync(WhatsAppWebhookRequest request);
    }
}
