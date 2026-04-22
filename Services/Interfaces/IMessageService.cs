
using ECommersAI.DTOs.Message;
using ECommersAI.Models.Entities;

namespace ECommersAI.Services.Interfaces
{
    public interface IMessageService
    {
        Task<List<MessageDto>> GetAllAsync();
        Task<MessageDto?> GetByIdAsync(Guid id);
        Task<Message> CreateAsync(Guid traderId, string customerPhone, string messageType, string content, string aiResponse);
        Task<List<Message>> GetMessageHistoryAsync(Guid traderId, string customerPhone, CancellationToken cancellationToken);
    }
}
