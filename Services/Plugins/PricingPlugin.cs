using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ECommersAI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace ECommersAI.Services.Plugins
{
    public class PricingPlugin
    {
        private static readonly IReadOnlyDictionary<string, string> SupportedCurrencies =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["YER_OLD"] = "ريال يمني (صنعاء)",
                ["YER_NEW"] = "ريال يمني (عدن)",
                ["SAR"] = "ريال سعودي"
            };

        private readonly IExchangeRateService _exchangeRateService;
        private readonly ILogger<PricingPlugin> _logger;

        public PricingPlugin(IExchangeRateService exchangeRateService, ILogger<PricingPlugin> logger)
        {
            _exchangeRateService = exchangeRateService;
            _logger = logger;
        }

        [KernelFunction("CalculateLocalPrice")]
        [Description("Calculates local currency price from USD for YER_OLD, YER_NEW, and SAR.")]
        public async Task<string> CalculateLocalPrice(double priceUsd, string currency)
        {
            if (priceUsd <= 0)
            {
                return "يرجى إدخال سعر صحيح أكبر من صفر.";
            }

            if (string.IsNullOrWhiteSpace(currency))
            {
                return "يرجى تحديد العملة المطلوبة: YER_OLD أو YER_NEW أو SAR.";
            }

            var normalizedCurrency = currency.Trim().ToUpperInvariant();
            if (!SupportedCurrencies.TryGetValue(normalizedCurrency, out var currencyLabel))
            {
                return "العملات المدعومة هي: YER_OLD و YER_NEW و SAR فقط.";
            }

            try
            {
                var convertedPrice = await _exchangeRateService.ConvertFromUsdAsync((decimal)priceUsd, normalizedCurrency);
                return $"السعر {priceUsd:0.##} دولار يساوي {convertedPrice:0.##} {currencyLabel}.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Price conversion failed for currency {Currency}", normalizedCurrency);
                return "حدث خطأ أثناء تحويل السعر. حاول مرة أخرى.";
            }
        }
    }
}
