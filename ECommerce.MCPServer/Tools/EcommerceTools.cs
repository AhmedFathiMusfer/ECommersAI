using System.ComponentModel;
using ECommerce.MCPServer.Models;
using ECommerce.MCPServer.Services;
using Microsoft.AspNetCore.Mvc;
using ModelContextProtocol.Server;

namespace ECommerce.MCPServer.Tools;

[McpServerToolType]
public sealed class EcommerceTools(IEcommerceProductService productService, ILogger<EcommerceTools> logger)
{
    [McpServerTool(Name = "search_products"), Description("Search for products by name or description.")]
    public async Task<IReadOnlyList<ProductSearchResult>> search_products(string query, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Searching for products with query: '{query}'");
        return await productService.SearchProductsAsync(query, cancellationToken);
    }

    [McpServerTool(Name = "get_product_price"), Description("Get the price of a product by its name.")]
    public async Task<string> get_product_price(string productName, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting price for product: '{productName}'");
        var product = await productService.GetProductByNameAsync(productName, cancellationToken);
        if (product is null)
        {
            return $"Product '{productName}' was not found.";
        }

        var availability = product.InStock ? "In stock" : "Out of stock";
        return $"{product.Name}: ${product.Price:0.00} ({availability}).";
    }
}
