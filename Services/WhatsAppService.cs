using System;
using System.Linq;
using System.Threading.Tasks;
using ECommersAI.DTOs.Message;
using ECommersAI.Services.Background;
using ECommersAI.Services.Interfaces;

namespace ECommersAI.Services
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IMessageService _messageService;
        private readonly IProductService _productService;
        private readonly IAIService _aiService;

        public WhatsAppService(
            IBackgroundTaskQueue taskQueue,
            IMessageService messageService,
            IProductService productService,
            IAIService aiService)
        {
            _taskQueue = taskQueue;
            _messageService = messageService;
            _productService = productService;
            _aiService = aiService;
        }

        public async Task QueueIncomingMessageAsync(WhatsAppWebhookRequest request)
        {
            await _taskQueue.QueueBackgroundWorkItemAsync(async _ =>
            {
                await ProcessIncomingMessageAsync(request);
            });
        }

        public async Task ProcessIncomingMessageAsync(WhatsAppWebhookRequest request)
        {
            var incomingText = request.Content;
            if (string.Equals(request.MessageType, "voice", StringComparison.OrdinalIgnoreCase))
            {
                incomingText = await _aiService.TranscribeVoiceAsync(request.MediaUrl ?? string.Empty);
            }

            var products = await _productService.SearchByQueryAsync(request.TraderId, incomingText, "USD", 3);
            var hints = products.Select(p => $"{p.ProductName} ({p.DisplayPrice} {p.Currency})");
            var aiReply = await _aiService.GenerateReplyAsync(incomingText, hints);

            await _messageService.CreateAsync(
                request.TraderId,
                request.CustomerPhone,
                request.MessageType,
                incomingText,
                aiReply);
        }
    }
}
