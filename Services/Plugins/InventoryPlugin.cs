using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ECommersAI.Data;
using ECommersAI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace ECommersAI.Services.Plugins
{
    public class InventoryPlugin
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAIService _aiService;
        private readonly ILogger<InventoryPlugin> _logger;
        private Guid _traderId;

        public InventoryPlugin(
            ApplicationDbContext dbContext,
            IAIService aiService,
            ILogger<InventoryPlugin> logger)
        {
            _dbContext = dbContext;
            _aiService = aiService;
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
                var embedding = await _aiService.GenerateEmbeddingAsync(query);
                var queryVector = new Vector(embedding);

                var products = await _dbContext.ProductVectors
                    .AsNoTracking()
                    .Where(v => v.Product.TraderId == _traderId)
                    .OrderBy(v => v.Vector.CosineDistance(queryVector))
                    .Take(5)
                    .Select(v => new
                    {
                        v.Product.Name,
                        v.Product.PriceUSD
                    })
                    .ToListAsync();

                if (products.Count == 0)
                {
                    return "لم أجد منتجات مطابقة حاليا.";
                }

                var lines = products
                    .Select((p, index) => $"{index + 1}) {p.Name} - {p.PriceUSD:0.##} USD")
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
