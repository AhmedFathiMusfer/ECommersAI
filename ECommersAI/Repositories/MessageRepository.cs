using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ECommersAI.Data;
using ECommersAI.Models.Entities;
using ECommersAI.Repositories.interfaces;

namespace ECommersAI.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;
        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> GetAllAsync()
        {
            return await _context.Messages.ToListAsync();
        }

        public async Task<Message> GetByIdAsync(Guid id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task AddAsync(Message entity)
        {
            await _context.Messages.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Message entity)
        {
            _context.Messages.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Message>> GetHistoryMessagesAsync(Guid TraderId, string CustomerPhone, CancellationToken cancellationToken)
        {            return  await _context.Messages
                .AsNoTracking()
                .Where(m => m.TraderId == TraderId && m.CustomerPhone == CustomerPhone)
                .OrderByDescending(m => m.CreatedAt)
                .Take(8)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync(cancellationToken);
        }


    }
}
