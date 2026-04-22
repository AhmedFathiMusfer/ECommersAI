using ECommersAI.Models.Entities;

namespace ECommersAI.Repositories.interfaces
{
    public interface ITraderRepository : IRepository<Trader>
    {
        Task<Trader?> GetByWhatsAppIdAsync(string whatsAppId);
    }
}