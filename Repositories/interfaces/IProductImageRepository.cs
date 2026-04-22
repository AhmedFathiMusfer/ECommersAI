using ECommersAI.Models.Entities;

namespace ECommersAI.Repositories.interfaces
{
    public interface IProductImageRepository : IRepository<ProductImage>
    {
        Task<IEnumerable<ProductImage>> GetByProductIdAsync(Guid productId);
        Task<ProductImage?> GetMainByProductIdAsync(Guid productId);
    }
}