using System.Threading;
using System.Threading.Tasks;
using ECommersAI.DTOs.AI;
using ECommersAI.DTOs.Message;
using ECommersAI.Features.AI.Agent;
using ECommersAI.Features.AI.DTOs;
using ECommersAI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ECommersAI.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AIController : ControllerBase
    {
        private readonly IAgentService _agentService;
        private readonly IMessageService _messageService;
        private readonly ILogger<AIController> _logger;

        public AIController(
            IAgentService agentService,
            IMessageService messageService,
            ILogger<AIController> logger)
        {
            _agentService = agentService;
            _messageService = messageService;
            _logger = logger;
        }

        [HttpPost("chat")]
        public async Task<ActionResult<ChatResponseDto>> Chat([FromBody] MessageRequestDto request, CancellationToken cancellationToken)
        {
            if (request.TraderId == System.Guid.Empty)
            {
                return BadRequest(new { error = "TraderId is required." });
            }

            if (string.IsNullOrWhiteSpace(request.CustomerPhone))
            {
                return BadRequest(new { error = "CustomerPhone is required." });
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { error = "Message is required." });
            }

            try
            {
                var responseText = await _agentService.SendAsync(request, cancellationToken);

                await _messageService.CreateAsync(
                    request.TraderId,
                    request.CustomerPhone,
                    "text",
                    request.Message,
                    responseText);

                return Ok(new ChatResponseDto
                {
                    Response = responseText
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "AI chat endpoint failed for trader {TraderId}", request.TraderId);
                return StatusCode(500, new { error = "حدث خطأ داخلي أثناء معالجة طلب الذكاء الاصطناعي." });
            }
        }
    }
}
