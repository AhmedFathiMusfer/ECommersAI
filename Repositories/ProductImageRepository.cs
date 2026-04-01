using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ECommersAI.Data;
using ECommersAI.Models.Entities;

namespace ECommersAI.Repositories
{
    public class ProductImageRepository : IRepository<ProductImage>
    {
        private readonly ApplicationDbContext _context;
        public ProductImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductImage>> GetAllAsync()
        {
            return await _context.ProductImages.ToListAsync();
        }

        public async Task<ProductImage> GetByIdAsync(Guid id)
        {
            return await _context.ProductImages.FindAsync(id);
        }

        public async Task AddAsync(ProductImage entity)
        {
            await _context.ProductImages.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductImage entity)
        {
            _context.ProductImages.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var image = await _context.ProductImages.FindAsync(id);
            if (image != null)
            {
                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();
            }
        }
    }
}
