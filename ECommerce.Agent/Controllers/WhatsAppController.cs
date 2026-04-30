using ECommerce.Agent.Services.Interface;
using ECommerce.Agent.Configurations.Options;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


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


                backgroundJobClient.Enqueue<IWatsAppService>(s => s.ProcessAndReplyAsync(from, text));
            }
        }
        catch { }

        return Ok();
    }


}
public class WhatsAppWebhookDto
{
    public string? Event { get; set; }
    public string? SessionId { get; set; }
    public WhatsAppData? Data { get; set; }
    public long Timestamp { get; set; }
}

public class WhatsAppData
{
    public WhatsAppMessageWrapper? Messages { get; set; }
}

public class WhatsAppMessageWrapper
{
    public WhatsAppKey? Key { get; set; }
    public long MessageTimestamp { get; set; }
    public string? PushName { get; set; }
    public bool Broadcast { get; set; }
    public WhatsAppMessage? Message { get; set; }
    public string? MessageBody { get; set; }
    public string? RemoteJid { get; set; }
    public string? Id { get; set; }
}

public class WhatsAppKey
{
    public string? Id { get; set; }
    public bool FromMe { get; set; }
    public string? RemoteJid { get; set; }
    public string? SenderPn { get; set; }
    public string? CleanedSenderPn { get; set; }
    public string? SenderLid { get; set; }
    public string? AddressingMode { get; set; }
}

public class WhatsAppMessage
{
    public ExtendedTextMessage? ExtendedTextMessage { get; set; }
}

public class ExtendedTextMessage
{
    public string? Text { get; set; }
}