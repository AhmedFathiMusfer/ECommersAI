using ECommerce.MCPServer.Models;

namespace ECommerce.MCPServer.Services;

public sealed class DummyEcommerceProductService : IEcommerceProductService
{
    private static readonly IReadOnlyList<Product> Products =
    [
        new Product("iPhone 15", 999.99m, "Apple smartphone with A16 chip", true),
        new Product("Samsung Galaxy S24", 899.00m, "Flagship Android phone", true),
        new Product("Sony WH-1000XM5", 349.50m, "Noise-cancelling over-ear headphones", false),
        new Product("Logitech MX Master 3S", 99.99m, "Wireless productivity mouse", true),
        new Product("Dell XPS 13", 1299.00m, "Premium ultrabook laptop", false)
    ];


    public Task<IReadOnlyList<ProductSearchResult>> SearchProductsAsync(string query, CancellationToken cancellationToken = default)
    {
       
        var normalizedQuery = query.Trim();
        if (string.IsNullOrWhiteSpace(normalizedQuery))
        {
            return Task.FromResult<IReadOnlyList<ProductSearchResult>>(Products.Select(p => new ProductSearchResult(p.Name, p.Price, p.Description, p.InStock))
            .ToList());
        }

        var result = Products
            .Where(p =>
                p.Name.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase))
            .Select(p => new ProductSearchResult(p.Name, p.Price, p.Description, p.InStock))
            .ToList();

        return Task.FromResult<IReadOnlyList<ProductSearchResult>>(result);
    }

    public Task<Product?> GetProductByNameAsync(string productName, CancellationToken cancellationToken = default)
    {
       
        var product = Products.FirstOrDefault(p =>
            p.Name.Equals(productName.Trim(), StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(product);
    }
}
