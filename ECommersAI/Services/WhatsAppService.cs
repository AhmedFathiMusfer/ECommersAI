using System;
using System.Linq;
using System.Threading.Tasks;
using ECommersAI.DTOs.AI;
using ECommersAI.DTOs.Message;
using ECommersAI.Features.AI.Agent;
using ECommersAI.Features.AI.DTOs;
using ECommersAI.Features.AI.Embedding;
using ECommersAI.Services.Interfaces;
using Hangfire;

namespace ECommersAI.Services
{
    public class WhatsAppService : IWhatsAppService
    {
        //  private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IMessageService _messageService;
        private readonly IProductService _productService;

        private readonly IEmbeddingService _embeddingService;
        private readonly IAgentService _AgentService;

        public WhatsAppService(
            // IBackgroundJobClient backgroundJobClient,
            IMessageService messageService,
            IProductService productService,
            IAgentService agentService
            , IEmbeddingService embeddingService
           )
        {
            //   _backgroundJobClient = backgroundJobClient;
            _messageService = messageService;
            _productService = productService;
            _AgentService = agentService;
            _embeddingService = embeddingService;
        }

        public async Task QueueIncomingMessageAsync(WhatsAppWebhookRequest request)
        {
            // _backgroundJobClient.Enqueue<IWhatsAppService>(service => service.ProcessIncomingMessageAsync(request));
            // await Task.CompletedTask;
        }

        public async Task ProcessIncomingMessageAsync(WhatsAppWebhookRequest request)
        {
            // var incomingText = request.Content;
            // if (string.Equals(request.MessageType, "voice", StringComparison.OrdinalIgnoreCase))
            // {
            //     incomingText = await _embeddingService.TranscribeVoiceAsync(request.MediaUrl ?? string.Empty);
            // }
            // var result = await _AgentService.SendAsync(new MessageRequestDto
            // {
            //     TraderId = request.TraderId,
            //     CustomerPhone = request.CustomerPhone,
            //     Message = incomingText
            // });
            // var products = await _productService.SearchByQueryAsync(request.TraderId, incomingText, "USD", 3);
            // var hints = products.Select(p => $"{p.ProductName} ({p.DisplayPrice} {p.Currency})");
            // var aiReply = await _embeddingService.GenerateEmbeddingAsync(incomingText);

            // await _messageService.CreateAsync(
            //     request.TraderId,
            //     request.CustomerPhone,
            //     request.MessageType,
            //     incomingText,
            //     aiRep);
        }
    }
}
