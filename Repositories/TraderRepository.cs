using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ECommersAI.Data;
using ECommersAI.Models.Entities;
using ECommersAI.Repositories.interfaces;

namespace ECommersAI.Repositories
{
    public class TraderRepository : ITraderRepository
    {
        private readonly ApplicationDbContext _context;
        public TraderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Trader>> GetAllAsync()
        {
            return await _context.Traders.ToListAsync();
        }

        public async Task<Trader> GetByIdAsync(Guid id)
        {
            return await _context.Traders.FindAsync(id);
        }

        public async Task AddAsync(Trader entity)
        {
            await _context.Traders.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Trader entity)
        {
            _context.Traders.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var trader = await _context.Traders.FindAsync(id);
            if (trader != null)
            {
                _context.Traders.Remove(trader);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Trader?> GetByWhatsAppIdAsync(string whatsAppId)
        {
            return await _context.Traders
                .FirstOrDefaultAsync(t => t.WhatsAppId == whatsAppId);
        }
    }
}
