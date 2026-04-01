namespace ECommersAI.DTOs.Trader
{
    public class CreateTraderRequest
    {
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string WhatsAppId { get; set; } = string.Empty;
        public string SubscriptionStatus { get; set; } = "Trial";
        public string DefaultCurrency { get; set; } = "USD";
    }
}
