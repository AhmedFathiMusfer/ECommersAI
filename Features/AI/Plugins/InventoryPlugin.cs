using System.ComponentModel;
using ECommersAI.Services.Interfaces;
using Microsoft.SemanticKernel;

namespace ECommersAI.Features.AI.Plugins
{
    public class InventoryPlugin
    {
        private readonly IProductService _productService;
        private readonly ILogger<InventoryPlugin> _logger;
        private Guid _traderId;

        public InventoryPlugin(
            IProductService productService,
            ILogger<InventoryPlugin> logger)
        {
            _productService = productService;
            _logger = logger;
        }


        public void SetContext(Guid traderId)
        {
            _traderId = traderId;
        }

        [KernelFunction("SearchProducts")]
        [Description("Searches the trader inventory using semantic vector similarity and returns top matching products with USD prices.")]
        public async Task<string> SearchProducts(string query)
        {
            _logger.LogInformation(" SearchProducts     استخدام ");
            if (_traderId == Guid.Empty)
            {
                return "تعذر تحديد التاجر لهذا البحث.";
            }

            if (string.IsNullOrWhiteSpace(query))
            {
                return "يرجى كتابة وصف المنتج الذي تريد البحث عنه.";
            }

            try
            {
                var products = await _productService.SearchByVectorQueryAsync(_traderId, query);

                if (products.Count == 0)
                {
                    return "لم أجد منتجات مطابقة حاليا.";
                }

                var lines = products
                    .Select((p, index) => $"{index + 1}) {p.ProductName} - {p.PriceUSD:0.##} USD")
                    .ToList();

                return "أفضل المنتجات المطابقة:\n" + string.Join("\n", lines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Inventory search failed for trader {TraderId}", _traderId);
                return "حدث خطأ أثناء البحث عن المنتجات. حاول مرة أخرى.";
            }
        }
    }
}
