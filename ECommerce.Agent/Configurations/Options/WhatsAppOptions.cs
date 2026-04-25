namespace ECommerce.Agent.Configurations.Options;

public sealed class WhatsAppOptions
{
    public const string SectionName = "WhatsApp";

    public string VerifyToken { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string PhoneNumberId { get; set; } = string.Empty;
    public string GraphBaseUrl { get; set; } = "https://graph.facebook.com";
    public string ApiVersion { get; set; } = "v17.0";
    public string MessagingProduct { get; set; } = "whatsapp";
}
