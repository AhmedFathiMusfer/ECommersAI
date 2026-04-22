using ECommersAI.DTOs.AI;
using ECommersAI.Features.AI.DTOs;


namespace ECommersAI.Features.AI.Agent
{
    public interface IAgentService
    {

        Task<string> SendAsync(MessageRequestDto request, CancellationToken cancellationToken = default);
    }
}
