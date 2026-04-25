namespace ECommerce.Agent.Configurations.Options;

public sealed class McpOptions
{
    public const string SectionName = "Mcp";

    public string Name { get; set; } = "MCPServer";
    public string Endpoint { get; set; } = string.Empty;
}
