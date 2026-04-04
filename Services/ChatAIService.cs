
using ECommersAI.Configurations.Options;
using ECommersAI.Data;
using ECommersAI.DTOs.AI;
using ECommersAI.Services.Interfaces;
using ECommersAI.Services.Plugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.AspNetCore.OpenApi;

using System.ClientModel;
using Microsoft.SemanticKernel.Agents;
using OpenAI;

namespace ECommersAI.Services
{
    public class ChatAIService : IChatAIService
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

        private readonly ChatAIOptions _chatAiOptions;
        private readonly InventoryPlugin _inventoryPlugin;
        private readonly PricingPlugin _pricingPlugin;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ChatAIService> _logger;


        public ChatAIService(
            IOptions<ChatAIOptions> chatAiOptions,
            InventoryPlugin inventoryPlugin,
            PricingPlugin pricingPlugin,
            ApplicationDbContext dbContext,
            ILogger<ChatAIService> logger
)
        {
            _chatAiOptions = chatAiOptions.Value;
            _inventoryPlugin = inventoryPlugin;
            _pricingPlugin = pricingPlugin;
            _dbContext = dbContext;
            _logger = logger;

        }

        public async Task<string> ChatAsync(ChatRequestDto request, CancellationToken cancellationToken = default)
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
                // _inventoryPlugin.

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
                _logger.LogError(ex, "AI chat execution failed for trader {TraderId}, customer {CustomerPhone}", request.TraderId, request.CustomerPhone);
                return "حدث خطأ مؤقت أثناء معالجة طلبك. حاول مرة أخرى بعد قليل.";
            }
        }

        private Kernel BuildKernel()
        {
            var builder = Kernel.CreateBuilder();

            var opnAIOption = new OpenAIClientOptions()
            {
                Endpoint = new Uri(_chatAiOptions.BaseUrl),
            };
            var credentials = new ApiKeyCredential(_chatAiOptions.ApiKey);
            var ghModelsClinet = new OpenAIClient(credentials, opnAIOption);

            builder.AddOpenAIChatCompletion(
            _chatAiOptions.Model,
             ghModelsClinet
               );

            return builder.Build();
        }


        private static bool IsRateLimitException(Exception ex)
        {
            if (ex is HttpOperationException operationException)
            {
                if (operationException.InnerException is HttpRequestException httpRequestException &&
                    httpRequestException.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    return true;
                }

                if (operationException.Message.Contains("429", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return ex is HttpRequestException requestException &&
                   requestException.StatusCode == System.Net.HttpStatusCode.TooManyRequests;
        }

        private async Task<ChatHistory> BuildHistoryAsync(ChatRequestDto request, CancellationToken cancellationToken)
        {
            var history = new ChatHistory();
            history.AddSystemMessage(SystemPrompt);

            var latestMessages = await _dbContext.Messages
                .AsNoTracking()
                .Where(m => m.TraderId == request.TraderId && m.CustomerPhone == request.CustomerPhone)
                .OrderByDescending(m => m.CreatedAt)
                .Take(8)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync(cancellationToken);

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
