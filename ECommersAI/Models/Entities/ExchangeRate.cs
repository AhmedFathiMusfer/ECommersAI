using System;

namespace ECommersAI.Models.Entities
{
    public class ExchangeRate
    {
        public Guid Id { get; set; }
        public string Currency { get; set; }
        public decimal RateToUSD { get; set; }
        public DateTime Date { get; set; }
    }
}