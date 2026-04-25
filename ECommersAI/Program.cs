
using ECommersAI.Configurations;
using ECommersAI.Configurations.Options;
using ECommersAI.Data;
using ECommersAI.Features.AI.Agent;
using ECommersAI.Features.AI.Embedding;
using ECommersAI.Features.AI.Options;
using ECommersAI.Features.AI.Plugins;
using ECommersAI.Models.Entities;
using ECommersAI.Repositories;
using ECommersAI.Repositories.interfaces;
using ECommersAI.Services;
using ECommersAI.Services.Interfaces;

using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Pgvector.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

// // 🔥 تسجيل pgvector داخل Npgsql
// dataSourceBuilder.UseVector();

//var dataSource = dataSourceBuilder.Build();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, options => options.UseVector()));

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.Configure<AgentAIOptions>(builder.Configuration.GetSection("AgentAI"));
builder.Services.Configure<EmbeddingAIOption>(builder.Configuration.GetSection("EmbeddingAI"));

builder.Services.Configure<WhatsAppOptions>(builder.Configuration.GetSection("WhatsApp"));



builder.Services.AddHangfire(configuration => configuration
    .UseRecommendedSerializerSettings()
    .UseSimpleAssemblyNameTypeSerializer()
    .UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(connectionString);
    }));
builder.Services.AddHangfireServer();


builder.Services.AddScoped<ITraderRepository, TraderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IProductVectorRepository, ProductVectorRepository>();

builder.Services.AddScoped<ITraderService, TraderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddScoped<InventoryPlugin>();
builder.Services.AddScoped<PricingPlugin>();
builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();

builder.Services.AddHttpClient<IEmbeddingService, EmbeddingService>();
//builder.Services.AddHttpClient<IAgentService, ChatAIService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});
}

app.UseHttpsRedirection();

//app.UseHangfireDashboard("/hangfire");

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.EnsureSeededAsync(db);
}

app.Run();
