
using ECommersAI.DTOs.AI;

namespace ECommersAI.Services.Interfaces
{
    public interface IChatAIService
    {
        Task<string> ChatAsync(ChatRequestDto request, CancellationToken cancellationToken = default);
    }
}
