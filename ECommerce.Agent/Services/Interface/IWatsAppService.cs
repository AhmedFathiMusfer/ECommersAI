

namespace ECommerce.Agent.Services.Interface
{
    public interface IWatsAppService
    {
        Task ProcessAndReplyAsync(string phone, string text);
    }
}