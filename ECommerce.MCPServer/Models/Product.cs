namespace ECommerce.MCPServer.Models;

public sealed record Product(
    string Name,
    decimal Price,
    string Description,
    bool InStock);
