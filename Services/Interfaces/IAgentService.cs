using System.Threading;
using System.Threading.Tasks;
using ECommersAI.DTOs.AI;

namespace ECommersAI.Services.Interfaces
{
    public interface IAgentService
    {
        Task<string> ChatAsync(ChatRequestDto request, CancellationToken cancellationToken = default);
    }
}
