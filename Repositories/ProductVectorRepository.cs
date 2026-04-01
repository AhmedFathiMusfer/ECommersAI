using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ECommersAI.Data;
using ECommersAI.Models.Entities;

namespace ECommersAI.Repositories
{
    public class ProductVectorRepository : IRepository<ProductVector>
    {
        private readonly ApplicationDbContext _context;
        public ProductVectorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductVector>> GetAllAsync()
        {
            return await _context.ProductVectors.ToListAsync();
        }

        public async Task<ProductVector> GetByIdAsync(Guid id)
        {
            return await _context.ProductVectors.FirstOrDefaultAsync(v => v.ProductId == id);
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
    }
}
