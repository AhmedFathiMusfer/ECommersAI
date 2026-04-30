

namespace ECommerce.Agent.Services.Interface
{
    public interface IWhatsAppService
    {
        Task ProcessAndReplyAsync(string phone, string text);
    }
}