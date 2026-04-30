using ECommerce.Agent.Configurations.Options;
using ECommerce.Agent.Services.Interface;
using Microsoft.Extensions.Options;



namespace ECommerce.Agent.Services
{
    public class WatsAppService(IAgentService agentService, HttpClient httpClient, IOptions<WhatsAppOptions> whatsAppOptions) : IWatsAppService
    {
        public async Task ProcessAndReplyAsync(string phone, string text)
        {
            var result = await agentService.AgentChat(text);
            await SendToMetaAsync(phone, result);

        }

        private async Task SendToMetaAsync(string to, string message)
        {
            var url = whatsAppOptions.Value.GraphBaseUrl;
            var payload = new
            {

                to = to,

                text = message
            };

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", whatsAppOptions.Value.AccessToken);

            await httpClient.PostAsJsonAsync(url, payload);
        }
    }
}