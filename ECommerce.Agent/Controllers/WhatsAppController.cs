using ECommerce.Agent.Services.Interface;
using ECommerce.Agent.Configurations.Options;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

[ApiController]
[Route("api/whatsapp")]
public class WhatsAppController(IBackgroundJobClient backgroundJobClient, IOptions<WhatsAppOptions> whatsAppOptions) : ControllerBase
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
    public IActionResult Receive([FromBody] dynamic data)
    {
        try
        {
            var entry = data?.entry[0]?.changes[0]?.value;
            var message = entry?.messages?[0];

            if (message != null)
            {
                string phone = message.from;
                string text = message.text?.body ?? " ";


                backgroundJobClient.Enqueue<IAgentService>(s => s.ProcessAndReplyAsync(phone, text));
            }
        }
        catch { }

        return Ok();
    }


}