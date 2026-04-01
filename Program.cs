using ECommersAI.Actions;
using ECommersAI.Configurations;
using ECommersAI.Configurations.Options;
using ECommersAI.Data;
using ECommersAI.Models.Entities;
using ECommersAI.Repositories;
using ECommersAI.Services;
using ECommersAI.Services.Background;
using ECommersAI.Services.Interfaces;
using ECommersAI.Services.Plugins;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), o => o.UseVector()));

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.Configure<GeminiOptions>(builder.Configuration.GetSection("Gemini"));
builder.Services.Configure<WhatsAppOptions>(builder.Configuration.GetSection("WhatsApp"));

builder.Services.AddScoped<IRepository<Trader>, TraderRepository>();
builder.Services.AddScoped<IRepository<Product>, ProductRepository>();
builder.Services.AddScoped<IRepository<Order>, OrderRepository>();
builder.Services.AddScoped<IRepository<OrderItem>, OrderItemRepository>();
builder.Services.AddScoped<IRepository<ProductImage>, ProductImageRepository>();
builder.Services.AddScoped<IRepository<ProductAttribute>, ProductAttributeRepository>();
builder.Services.AddScoped<IRepository<ExchangeRate>, ExchangeRateRepository>();
builder.Services.AddScoped<IRepository<Message>, MessageRepository>();
builder.Services.AddScoped<IRepository<ProductVector>, ProductVectorRepository>();

builder.Services.AddScoped<ITraderService, TraderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
builder.Services.AddScoped<IAgentService, AIService>();
builder.Services.AddScoped<InventoryPlugin>();
builder.Services.AddScoped<PricingPlugin>();
builder.Services.AddScoped<IAIService, GeminiService>();
builder.Services.AddScoped<AutoGenerateOrderAction>();
builder.Services.AddHttpClient<IAIService, GeminiService>();
//builder.Services.AddHttpClient<IAgentService, AIService>();


builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<WhatsAppMessageHostedService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.EnsureSeededAsync(db);
}

app.Run();
