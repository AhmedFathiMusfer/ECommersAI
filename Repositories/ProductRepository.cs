using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ECommersAI.Data;
using ECommersAI.Models.Entities;
using ECommersAI.Repositories.interfaces;

namespace ECommersAI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Attributes)
                .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Attributes)
                .FirstOrDefaultAsync(p => p.Id == id) ?? throw new KeyNotFoundException("Product not found");
        }

        public async Task AddAsync(Product entity)
        {
            await _context.Products.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product entity)
        {
            _context.Products.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetByTraderIdAsync(Guid traderId)
        {
            return await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Attributes)
                .Where(p => p.TraderId == traderId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid traderId, string category)
        {
            return await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Attributes)
                .Where(p => p.TraderId == traderId && p.Category.ToLower() == category.ToLower())
                .ToListAsync();
        }
    }
}
