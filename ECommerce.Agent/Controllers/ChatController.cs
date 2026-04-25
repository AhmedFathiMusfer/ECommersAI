
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.Client;
using Microsoft.Extensions.Options;
using ECommerce.Agent.Configurations.Options;

namespace ECommerce.Agent.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController(Kernel kernel, Task<McpClient> mcpClient, IOptions<ChatOptions> chatOptions) : ControllerBase
    {
        private readonly ChatOptions _chatOptions = chatOptions.Value;

        [HttpPost]
        public async Task<IActionResult> Message([FromBody] ChatRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("Message is required");
            var mcp = await mcpClient;
            var tools = await mcp.ListToolsAsync();

            kernel.Plugins.AddFromFunctions("MCP_Server", tools.Select(aiFunction => aiFunction.AsKernelFunction()));

            var history = await BuildHistoryAsync(request, cancellationToken);
            var chatService = kernel.GetRequiredService<IChatCompletionService>();
            var settings = new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };

            var result = await chatService.GetChatMessageContentAsync(
                        history,
                        settings,
                        kernel,
                        cancellationToken);

            return Ok(new
            {
                response = result.Content ?? string.Empty
            });
        }
        private async Task<ChatHistory> BuildHistoryAsync(ChatRequest request, CancellationToken cancellationToken)
        {
            var history = new ChatHistory();
            history.AddSystemMessage(_chatOptions.SystemPrompt);

            // var latestMessages = await _messageService.GetMessageHistoryAsync(request.TraderId, request.CustomerPhone, cancellationToken);
            // foreach (var message in latestMessages)
            // {
            //     if (!string.IsNullOrWhiteSpace(message.Content))
            //     {
            //         history.AddUserMessage(message.Content);
            //     }

            //     if (!string.IsNullOrWhiteSpace(message.AIResponse))
            //     {
            //         history.AddAssistantMessage(message.AIResponse);
            //     }
            // }

            history.AddUserMessage(request.Message.Trim());
            return history;
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = default!;
    }

}