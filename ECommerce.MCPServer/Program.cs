using System.Text.Json;
using ECommerce.MCPServer.Services;
using ECommerce.MCPServer.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddMcpServer()
	.WithTools<EcommerceTools>()
	.WithHttpTransport(options =>
	{
		// Enable legacy SSE endpoints (/sse and /message) for HTTP + SSE clients.
		options.EnableLegacySse = true;
	});

builder.Services.AddSingleton<IEcommerceProductService, DummyEcommerceProductService>();

var app = builder.Build();

app.MapGet("/", () => "ECommerce MCP Server is running.");
app.MapMcp("/mcp");

app.Run();
