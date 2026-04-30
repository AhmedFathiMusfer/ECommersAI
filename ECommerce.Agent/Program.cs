using Microsoft.SemanticKernel;
using ModelContextProtocol.Client;
using Hangfire;
using OpenAI;
using Hangfire.MemoryStorage;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

using ECommerce.Agent.Configurations.Options;
using ECommerce.Agent.Services.Interface;
using ECommerce.Agent.Services;
using System.ClientModel;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<McpOptions>(builder.Configuration.GetSection(McpOptions.SectionName));
builder.Services.Configure<OpenAIOptions>(builder.Configuration.GetSection(OpenAIOptions.SectionName));
builder.Services.Configure<WhatsAppOptions>(builder.Configuration.GetSection(WhatsAppOptions.SectionName));
builder.Services.Configure<ChatOptions>(builder.Configuration.GetSection(ChatOptions.SectionName));

builder.Services.AddHangfire(config => config.UseMemoryStorage());
builder.Services.AddHangfireServer();
builder.Services.AddHttpClient<AgentService>();

builder.Services.AddTransient<Task<McpClient>>(sp =>
{
    var mcpOptions = sp.GetRequiredService<IOptions<McpOptions>>().Value;

    var transport = new HttpClientTransport(new HttpClientTransportOptions
    {

        Name = mcpOptions.Name,
        Endpoint = new Uri(mcpOptions.Endpoint)
    });

    return McpClient.CreateAsync(transport);
});

builder.Services.AddScoped(sp =>
{
    var openAISettings = sp.GetRequiredService<IOptions<OpenAIOptions>>().Value;

    var kernelBuilder = Kernel.CreateBuilder();
    var openAiOptions = new OpenAIClientOptions
    {
        Endpoint = new Uri(openAISettings.Endpoint)
    };
    var credentials = new ApiKeyCredential(openAISettings.ApiKey);
    var modelClient = new OpenAIClient(credentials, openAiOptions);

    kernelBuilder.AddOpenAIChatCompletion(openAISettings.ChatModel, modelClient);

    return kernelBuilder.Build();
});
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHangfireDashboard();
app.MapControllers();
app.Run();