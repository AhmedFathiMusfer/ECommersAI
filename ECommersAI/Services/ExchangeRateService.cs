using System;
using System.Linq;
using System.Threading.Tasks;
using ECommersAI.Models.Entities;
using ECommersAI.Repositories;
using ECommersAI.Repositories.interfaces;
using ECommersAI.Services.Interfaces;

namespace ECommersAI.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IExchangeRateRepository _exchangeRateRepository;

        public ExchangeRateService(IExchangeRateRepository exchangeRateRepository)
        {
            _exchangeRateRepository = exchangeRateRepository;
        }

        public async Task<decimal> GetRateToUsdAsync(string currency)
        {
            if (string.Equals(currency, "USD", StringComparison.OrdinalIgnoreCase))
            {
                return 1m;
            }

            var latestRate = await _exchangeRateRepository.GetLatestByCurrencyAsync(currency);

            return latestRate?.RateToUSD ?? 1m;
        }

        public async Task<decimal> ConvertFromUsdAsync(decimal usdPrice, string currency)
        {
            var rateToUsd = await GetRateToUsdAsync(currency);
            if (rateToUsd <= 0)
            {
                return usdPrice;
            }

            return Math.Round(usdPrice / rateToUsd, 2);
        }
    }
}
