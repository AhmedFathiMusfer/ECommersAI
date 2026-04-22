using System.Threading.Tasks;
using ECommersAI.DTOs.Message;
using ECommersAI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommersAI.Controllers
{
    [ApiController]
    [Route("webhooks/whatsapp")]
    public class WhatsAppWebhookController : ControllerBase
    {
        private readonly IWhatsAppService _whatsAppService;
        private readonly ILogger<WhatsAppWebhookController> _logger;

        public WhatsAppWebhookController(IWhatsAppService whatsAppService, ILogger<WhatsAppWebhookController> logger)
        {
            _whatsAppService = whatsAppService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Receive([FromBody] WhatsAppWebhookRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body is required.");
            }

            _logger.LogInformation("Received WhatsApp message for trader {TraderId} from {CustomerPhone}.", request.TraderId, request.CustomerPhone);
            await _whatsAppService.QueueIncomingMessageAsync(request);
            return Accepted(new { status = "queued" });
        }
        [HttpGet]
        [Route("verify")]
        public IActionResult Verify(
    [FromQuery(Name = "hub.mode")] string mode,
    [FromQuery(Name = "hub.verify_token")] string token,
    [FromQuery(Name = "hub.challenge")] string challenge)
        {
            if (mode == "subscribe" && token == "rEAAguB9pdo2UBRDZCUazWfbhegsgsggsgsaaa")
                return Ok(challenge);

            return Forbid();
        }
    }
}
