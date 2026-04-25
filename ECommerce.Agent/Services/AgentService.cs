
using ECommerce.Agent.Services.Interface;
using ECommerce.Agent.Configurations.Options;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace ECommerce.Agent.Services;

public class AgentService(Kernel kernel, HttpClient httpClient, IOptions<WhatsAppOptions> whatsAppOptions) : IAgentService
{
    private readonly WhatsAppOptions _whatsAppOptions = whatsAppOptions.Value;

    public async Task ProcessAndReplyAsync(string phone, string text)
    {
        // 1. إعدادات الـ AI للتعامل مع أدوات الـ MCP تلقائياً
        var settings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        // 2. معالجة الطلب (قد يستغرق وقت، هانج فاير سيتولى الأمر)
        var result = await kernel.InvokePromptAsync(text, new(settings));

        // 3. إرسال الرد النهائي لواتساب
        await SendToMetaAsync(phone, result.ToString());
    }

    private async Task SendToMetaAsync(string to, string message)
    {
        var url = $"{_whatsAppOptions.GraphBaseUrl}/{_whatsAppOptions.ApiVersion}/{_whatsAppOptions.PhoneNumberId}/messages";
        var payload = new
        {
            messaging_product = _whatsAppOptions.MessagingProduct,
            to = to,
            type = "text",
            text = new { body = message }
        };

        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _whatsAppOptions.AccessToken);

        await httpClient.PostAsJsonAsync(url, payload);
    }
}