using ECommersAI.Data;
using ECommersAI.DTOs.AI;
using ECommersAI.Features.AI.DTOs;
using ECommersAI.Features.AI.Options;
using ECommersAI.Features.AI.Plugins;
using ECommersAI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI;
using System.ClientModel;

namespace ECommersAI.Features.AI.Agent
{
    public class AgentService : IAgentService
    {
        private const string SystemPrompt = """
You are a smart Yemeni e-commerce assistant.
You help users find products and calculate prices.

Rules:
- Use InventoryPlugin for product search queries.
- Use PricingPlugin for price or currency queries.
- Always respond in Arabic.
- If user refers to previous product, maintain context.
""";

        private readonly AgentAIOptions _chatAiOptions;
        private readonly InventoryPlugin _inventoryPlugin;
        private readonly PricingPlugin _pricingPlugin;
        private readonly IMessageService _messageService;
        private readonly ILogger<AgentService> _logger;

        public AgentService(
            IOptions<AgentAIOptions> chatAiOptions,
            InventoryPlugin inventoryPlugin,
            PricingPlugin pricingPlugin,
            IMessageService messageService,
            ILogger<AgentService> logger)
        {
            _chatAiOptions = chatAiOptions.Value;
            _inventoryPlugin = inventoryPlugin;
            _pricingPlugin = pricingPlugin;
            _messageService = messageService;

            _logger = logger;
        }



        public async Task<string> SendAsync(MessageRequestDto request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return "يرجى كتابة رسالتك أولا.";
            }

            if (string.IsNullOrWhiteSpace(_chatAiOptions.ApiKey))
            {
                return "خدمة الذكاء الاصطناعي غير مفعلة حاليا بسبب إعدادات المفتاح.";
            }

            try
            {
                _inventoryPlugin.SetContext(request.TraderId);

                var kernel = BuildKernel();
                kernel.Plugins.AddFromObject(_inventoryPlugin, "InventoryPlugin");
                kernel.Plugins.AddFromObject(_pricingPlugin, "PricingPlugin");

                var history = await BuildHistoryAsync(request, cancellationToken);
                var chatService = kernel.GetRequiredService<IChatCompletionService>();
                var executionSettings = new OpenAIPromptExecutionSettings
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                    Temperature = 0.2,
                    MaxTokens = 400
                };

                var result = await chatService.GetChatMessageContentAsync(
                    history,
                    executionSettings,
                    kernel,
                    cancellationToken);

                return result.Content ?? "عذرا، لم أتمكن من توليد رد في الوقت الحالي.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI agent execution failed for trader {TraderId}, customer {CustomerPhone}", request.TraderId, request.CustomerPhone);
                return "حدث خطأ مؤقت أثناء معالجة طلبك. حاول مرة أخرى بعد قليل.";
            }
        }

        private Kernel BuildKernel()
        {
            var builder = Kernel.CreateBuilder();

            var openAiOptions = new OpenAIClientOptions
            {
                Endpoint = new Uri(_chatAiOptions.BaseUrl)
            };

            var credentials = new ApiKeyCredential(_chatAiOptions.ApiKey);
            var modelClient = new OpenAIClient(credentials, openAiOptions);

            builder.AddOpenAIChatCompletion(
                _chatAiOptions.Model,
                modelClient);

            return builder.Build();
        }

        private async Task<ChatHistory> BuildHistoryAsync(MessageRequestDto request, CancellationToken cancellationToken)
        {
            var history = new ChatHistory();
            history.AddSystemMessage(SystemPrompt);

            var latestMessages = await _messageService.GetMessageHistoryAsync(request.TraderId, request.CustomerPhone, cancellationToken);
            foreach (var message in latestMessages)
            {
                if (!string.IsNullOrWhiteSpace(message.Content))
                {
                    history.AddUserMessage(message.Content);
                }

                if (!string.IsNullOrWhiteSpace(message.AIResponse))
                {
                    history.AddAssistantMessage(message.AIResponse);
                }
            }

            history.AddUserMessage(request.Message.Trim());
            return history;
        }
    }
}
