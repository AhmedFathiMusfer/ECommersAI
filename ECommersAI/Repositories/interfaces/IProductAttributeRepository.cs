using ECommersAI.Models.Entities;

namespace ECommersAI.Repositories.interfaces
{
    public interface IProductAttributeRepository : IRepository<ProductAttribute>
    {
        Task<IEnumerable<ProductAttribute>> GetByProductIdAsync(Guid productId);
    }
}