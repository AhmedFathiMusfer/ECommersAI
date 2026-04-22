using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommersAI.Models.Entities;

namespace ECommersAI.Repositories.interfaces
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<List<Message>> GetHistoryMessagesAsync(Guid TraderId, string CustomerPhone, CancellationToken cancellationToken);
    }
}