using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ECommersAI.Data;
using ECommersAI.Models.Entities;

namespace ECommersAI.Repositories
{
    public class ProductAttributeRepository : IRepository<ProductAttribute>
    {
        private readonly ApplicationDbContext _context;
        public ProductAttributeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductAttribute>> GetAllAsync()
        {
            return await _context.ProductAttributes.ToListAsync();
        }

        public async Task<ProductAttribute> GetByIdAsync(Guid id)
        {
            return await _context.ProductAttributes.FindAsync(id);
        }

        public async Task AddAsync(ProductAttribute entity)
        {
            await _context.ProductAttributes.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductAttribute entity)
        {
            _context.ProductAttributes.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var attr = await _context.ProductAttributes.FindAsync(id);
            if (attr != null)
            {
                _context.ProductAttributes.Remove(attr);
                await _context.SaveChangesAsync();
            }
        }
    }
}
