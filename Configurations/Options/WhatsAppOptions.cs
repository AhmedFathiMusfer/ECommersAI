namespace ECommersAI.Configurations.Options
{
    public class WhatsAppOptions
    {
        public string VerifyToken { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string PhoneNumberId { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://graph.facebook.com/v20.0";
    }
}
