using ECommerce.Agent.Configurations.Options;
using ECommerce.Agent.Services.Interface;
using Microsoft.Extensions.Options;



namespace ECommerce.Agent.Services
{
    public class WhatsAppService(IAgentService agentService, HttpClient httpClient, IOptions<WhatsAppOptions> whatsAppOptions, ILogger<WhatsAppService> logger) : IWhatsAppService
    {
        public async Task ProcessAndReplyAsync(string phone, string text)
        {
            var result = await agentService.AgentChat(text);
            await SendToMetaAsync(phone, result);

        }

        private async Task SendToMetaAsync(string to, string message)
        {
            var url = whatsAppOptions.Value.GraphBaseUrl;
            var body = new
            {
                messaging_product = "whatsapp",
                to = to,
                type = "text",
                text = new
                {
                    body = message
                }
            };

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", whatsAppOptions.Value.AccessToken);

            var response = await httpClient.PostAsJsonAsync(url, body);
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation($"Message sent to {to} successfully.");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                logger.LogError($"Failed to send message to {to}. Status Code: {response.StatusCode}, Response: {errorContent}");
            }

        }
    }
}