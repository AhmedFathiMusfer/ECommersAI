using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ECommersAI.Data;
using ECommersAI.Models.Entities;
using ECommersAI.Repositories.interfaces;

namespace ECommersAI.Repositories
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        private readonly ApplicationDbContext _context;
        public ExchangeRateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExchangeRate>> GetAllAsync()
        {
            return await _context.ExchangeRates.ToListAsync();
        }

        public async Task<ExchangeRate> GetByIdAsync(Guid id)
        {
            return await _context.ExchangeRates.FindAsync(id);
        }

        public async Task AddAsync(ExchangeRate entity)
        {
            await _context.ExchangeRates.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ExchangeRate entity)
        {
            _context.ExchangeRates.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var rate = await _context.ExchangeRates.FindAsync(id);
            if (rate != null)
            {
                _context.ExchangeRates.Remove(rate);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ExchangeRate?> GetLatestByCurrencyAsync(string currency)
        {
            return await _context.ExchangeRates
                .Where(r => r.Currency.ToLower() == currency.ToLower())
                .OrderByDescending(r => r.Date)
                .FirstOrDefaultAsync();
        }
    }
}
