using ECommerce.Agent.Services.Interface;
using ECommerce.Agent.Configurations.Options;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ECommerce.Agent.Dto;
using System.Text.Json;


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


    // [HttpPost]
    // public IActionResult Receive([FromBody] WhatsAppWebhookDto data)
    // {
    //     try
    //     {
    //         var dataString = data?.ToString() ?? "null";
    //         logger.LogInformation($"Received WhatsApp message:{dataString} ");

    //         var message = data?.Data?.Messages;
    //         if (message != null)
    //         {
    //             var text = message.MessageBody ?? " ";
    //             var sender = message.PushName;
    //             var from = message.RemoteJid ?? " ";


    //             backgroundJobClient.Enqueue<IWhatsAppService>(s => s.ProcessAndReplyAsync(from, text));
    //         }
    //     }
    //     catch
    //     {
    //         logger.LogError("Error processing WhatsApp message");
    //     }

    //     return Ok();
    // }

    [HttpPost]
    public IActionResult Receive([FromBody] JsonElement data)
    {
        try
        {
            var dataString = data.ToString() ?? "null";
            logger.LogInformation($"Received WhatsApp message:{dataString} ");
            // var message = data?.entry[0].changes[0].value.messages[0].text.body;
            // var from = data?.entry[0].changes[0].value.messages[0].from;
            var entry = data.GetProperty("entry")[0];
            var changes = entry.GetProperty("changes")[0];
            var value = changes.GetProperty("value");

            if (value.TryGetProperty("messages", out var messagesElement))
            {
                var messages = messagesElement[0];
                if (messages.GetProperty("type").GetString() == "text")
                {
                    var text = messages.GetProperty("text").GetProperty("body").GetString();
                    var from = messages.GetProperty("from").GetString();
                    backgroundJobClient.Enqueue<IWhatsAppService>(s => s.ProcessAndReplyAsync(from, text));
                }
            }




        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error processing WhatsApp message {ex.Message}");
        }

        return Ok();
    }


}
