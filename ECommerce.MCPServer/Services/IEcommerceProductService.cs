using ECommerce.MCPServer.Models;

namespace ECommerce.MCPServer.Services;

public interface IEcommerceProductService
{
    Task<IReadOnlyList<ProductSearchResult>> SearchProductsAsync(string query, CancellationToken cancellationToken = default);
    Task<Product?> GetProductByNameAsync(string productName, CancellationToken cancellationToken = default);
}
