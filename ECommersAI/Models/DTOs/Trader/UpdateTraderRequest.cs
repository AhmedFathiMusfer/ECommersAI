namespace ECommersAI.DTOs.Trader
{
    public class UpdateTraderRequest
    {
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string WhatsAppId { get; set; } = string.Empty;
        public string SubscriptionStatus { get; set; } = "Active";
        public string DefaultCurrency { get; set; } = "USD";
    }
}
