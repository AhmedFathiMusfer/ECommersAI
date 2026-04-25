namespace ECommerce.MCPServer.Models;

/// <summary>
/// Represents the result of a product search, including name, price, description, and stock status.
/// </summary>
public sealed record ProductSearchResult(

    string Name,

    decimal Price,

    string Description,

    bool InStock);
