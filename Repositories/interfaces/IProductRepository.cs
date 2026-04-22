using ECommersAI.Models.Entities;

namespace ECommersAI.Repositories.interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetByTraderIdAsync(Guid traderId);
        Task<IEnumerable<Product>> GetByCategoryAsync(Guid traderId, string category);
    }
}