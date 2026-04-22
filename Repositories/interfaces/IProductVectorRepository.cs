using ECommersAI.Models.Entities;
using Pgvector;

namespace ECommersAI.Repositories
{
    public interface IProductVectorRepository : IRepository<ProductVector>
    {
        IQueryable<ProductVector> SearchNearestByTrader(Guid traderId, Vector queryVector);
    }
}