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

        public WhatsAppWebhookController(IWhatsAppService whatsAppService)
        {
            _whatsAppService = whatsAppService;
        }

        [HttpPost]
        public async Task<IActionResult> Receive(WhatsAppWebhookRequest request)
        {
            await _whatsAppService.QueueIncomingMessageAsync(request);
            return Accepted(new { status = "queued" });
        }
    }
}
