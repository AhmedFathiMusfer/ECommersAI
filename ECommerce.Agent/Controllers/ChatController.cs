
using Microsoft.AspNetCore.Mvc;
using ECommerce.Agent.Services.Interface;

namespace ECommerce.Agent.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController(IAgentService agentService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Message([FromBody] ChatRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("Message is required");
            var result = await agentService.AgentChat(request.Message);
            return Ok(new
            {
                response = result
            });
        }
        // private async Task<ChatHistory> BuildHistoryAsync(ChatRequest request, CancellationToken cancellationToken)
        // {
        //     var history = new ChatHistory();
        //     history.AddSystemMessage(_chatOptions.SystemPrompt);

        //     // var latestMessages = await _messageService.GetMessageHistoryAsync(request.TraderId, request.CustomerPhone, cancellationToken);
        //     // foreach (var message in latestMessages)
        //     // {
        //     //     if (!string.IsNullOrWhiteSpace(message.Content))
        //     //     {
        //     //         history.AddUserMessage(message.Content);
        //     //     }

        //     //     if (!string.IsNullOrWhiteSpace(message.AIResponse))
        //     //     {
        //     //         history.AddAssistantMessage(message.AIResponse);
        //     //     }
        //     // }

        //     history.AddUserMessage(request.Message.Trim());
        //     return history;
        // }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = default!;
    }

}