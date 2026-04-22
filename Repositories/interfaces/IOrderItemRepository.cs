using ECommersAI.Models.Entities;

namespace ECommersAI.Repositories.interfaces
{
    public interface IOrderItemRepository : IRepository<OrderItem>
    {
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(Guid orderId);
    }
}