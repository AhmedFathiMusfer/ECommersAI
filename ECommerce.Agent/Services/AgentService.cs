
using ECommerce.Agent.Services.Interface;
using ECommerce.Agent.Configurations.Options;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.Client;
using Microsoft.SemanticKernel.ChatCompletion;

namespace ECommerce.Agent.Services;

public class AgentService(Kernel kernel, Task<McpClient> mcpClient, IOptions<ChatOptions> chatOptions) : IAgentService
{
    public async Task<string> AgentChat(string message)
    {
        var mcp = await mcpClient;
        var tools = await mcp.ListToolsAsync();

        kernel.Plugins.AddFromFunctions("MCP_Server", tools.Select(aiFunction => aiFunction.AsKernelFunction()));
        var history = new ChatHistory();
        history.AddSystemMessage(chatOptions.Value.SystemPrompt);
        history.AddUserMessage(message.Trim());

        var chatService = kernel.GetRequiredService<IChatCompletionService>();
        var settings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        var result = await chatService.GetChatMessageContentAsync(
                    history,
                    settings,
                    kernel,
                    CancellationToken.None);
        return result.Content ?? String.Empty;
    }




}