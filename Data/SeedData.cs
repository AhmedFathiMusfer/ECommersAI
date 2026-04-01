using System;
using System.Linq;
using System.Threading.Tasks;
using ECommersAI.Models.Entities;

namespace ECommersAI.Data
{
    public static class SeedData
    {
        public static async Task EnsureSeededAsync(ApplicationDbContext context)
        {
            if (context.Traders.Any())
            {
                return;
            }

            var traderId = Guid.NewGuid();
            var product1Id = Guid.NewGuid();
            var product2Id = Guid.NewGuid();
            var now = DateTime.UtcNow;

            context.Traders.Add(new Trader
            {
                Id = traderId,
                Name = "Yemen Fashion Store",
                PhoneNumber = "+967700000001",
                WhatsAppId = "967700000001",
                SubscriptionStatus = "Active",
                DefaultCurrency = "YER",
                CreatedAt = now,
                UpdatedAt = now
            });

            context.Products.AddRange(
                new Product
                {
                    Id = product1Id,
                    TraderId = traderId,
                    Name = "Cotton Shirt",
                    Description = "Men cotton shirt, breathable and soft",
                    PriceUSD = 15m,
                    Category = "Clothing",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Product
                {
                    Id = product2Id,
                    TraderId = traderId,
                    Name = "Leather Sandal",
                    Description = "Traditional sandal with durable sole",
                    PriceUSD = 20m,
                    Category = "Footwear",
                    CreatedAt = now,
                    UpdatedAt = now
                });

            context.ExchangeRates.AddRange(
                new ExchangeRate
                {
                    Id = Guid.NewGuid(),
                    Currency = "USD",
                    RateToUSD = 1m,
                    Date = now.Date
                },
                new ExchangeRate
                {
                    Id = Guid.NewGuid(),
                    Currency = "YER",
                    RateToUSD = 250m,
                    Date = now.Date
                },
                new ExchangeRate
                {
                    Id = Guid.NewGuid(),
                    Currency = "YER_OLD",
                    RateToUSD = 250m,
                    Date = now.Date
                },
                new ExchangeRate
                {
                    Id = Guid.NewGuid(),
                    Currency = "YER_NEW",
                    RateToUSD = 520m,
                    Date = now.Date
                },
                new ExchangeRate
                {
                    Id = Guid.NewGuid(),
                    Currency = "SAR",
                    RateToUSD = 0.27m,
                    Date = now.Date
                });

            await context.SaveChangesAsync();
        }
    }
}
