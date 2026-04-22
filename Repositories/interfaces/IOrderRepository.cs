using ECommersAI.Models.Entities;

namespace ECommersAI.Repositories.interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetByTraderIdAsync(Guid traderId);
    }
}