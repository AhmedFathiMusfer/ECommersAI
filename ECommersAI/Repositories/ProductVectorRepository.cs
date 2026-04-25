using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ECommersAI.Data;
using ECommersAI.Models.Entities;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace ECommersAI.Repositories
{
    public class ProductVectorRepository : IProductVectorRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductVectorRepository> _logger;

        public ProductVectorRepository(ApplicationDbContext context, ILogger<ProductVectorRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductVector>> GetAllAsync()
        {
            return await _context.ProductVectors.ToListAsync();
        }

        public async Task<ProductVector> GetByIdAsync(Guid id)
        {
            return await _context.ProductVectors.FirstOrDefaultAsync(v => v.ProductId == id) ?? throw new KeyNotFoundException("Product vector not found");
        }

        public async Task AddAsync(ProductVector entity)
        {
            await _context.ProductVectors.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductVector entity)
        {
            _context.ProductVectors.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var vector = await _context.ProductVectors.FirstOrDefaultAsync(v => v.ProductId == id);
            if (vector != null)
            {
                _context.ProductVectors.Remove(vector);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<ProductVector> SearchNearestByTrader(Guid traderId, Vector queryVector)
        {
            _logger.LogInformation("Searching for nearest product vectors by trader.");
            _logger.LogInformation($"Database connection type: {_context.Database.GetDbConnection().GetType()}");
            return _context.ProductVectors
                .AsNoTracking()
                .Where(v => v.Product.TraderId == traderId)
                // .Where(v => v.Vector != null)
                .Include(v => v.Product)
                .OrderBy(v => v.Vector.CosineDistance(queryVector));
        }
    }
}
