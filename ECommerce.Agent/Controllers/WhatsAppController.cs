using ECommerce.Agent.Services.Interface;
using ECommerce.Agent.Configurations.Options;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ECommerce.Agent.Dto;


[ApiController]
[Route("api/whatsapp")]
public class WhatsAppController(IBackgroundJobClient backgroundJobClient, IOptions<WhatsAppOptions> whatsAppOptions, ILogger<WhatsAppController> logger) : ControllerBase
{
    private readonly WhatsAppOptions _whatsAppOptions = whatsAppOptions.Value;

    [HttpGet]
    public IActionResult VerifyWebhook([FromQuery(Name = "hub.mode")] string mode,
                                     [FromQuery(Name = "hub.verify_token")] string token,
                                     [FromQuery(Name = "hub.challenge")] string challenge)
    {
        if (mode == "subscribe" && token == _whatsAppOptions.VerifyToken) return Ok(challenge);
        return Forbid();
    }


    [HttpPost]
    public IActionResult Receive([FromBody] WhatsAppWebhookDto data)
    {
        try
        {
            var dataString = data?.ToString() ?? "null";
            logger.LogInformation($"Received WhatsApp message:{dataString} ");

            var message = data?.Data?.Messages;
            if (message != null)
            {
                var text = message.MessageBody ?? " ";
                var sender = message.PushName;
                var from = message.RemoteJid ?? " ";


                backgroundJobClient.Enqueue<IWhatsAppService>(s => s.ProcessAndReplyAsync(from, text));
            }
        }
        catch
        {
            logger.LogError("Error processing WhatsApp message");
        }

        return Ok();
    }


}
