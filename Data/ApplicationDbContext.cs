using Microsoft.EntityFrameworkCore;
using ECommersAI.Models.Entities;

namespace ECommersAI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Trader> Traders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ProductVector> ProductVectors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // pgvector extension
            modelBuilder.HasPostgresExtension("vector");

            // ProductVector config for pgvector (1536-dim)
            modelBuilder.Entity<ProductVector>()
                .HasKey(v => v.ProductId);

            // ProductVector config for pgvector (1536-dim)
            modelBuilder.Entity<ProductVector>()
                .Property(v => v.Vector)
                .HasColumnType("vector(1536)");

            // Relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductVector)
                .WithOne(v => v.Product)
                .HasForeignKey<ProductVector>(v => v.ProductId);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Attributes)
                .WithOne(a => a.Product)
                .HasForeignKey(a => a.ProductId);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId);

            modelBuilder.Entity<Trader>()
                .HasMany(t => t.Products)
                .WithOne(p => p.Trader)
                .HasForeignKey(p => p.TraderId);

            modelBuilder.Entity<Trader>()
                .HasMany(t => t.Orders)
                .WithOne(o => o.Trader)
                .HasForeignKey(o => o.TraderId);

            modelBuilder.Entity<Trader>()
                .HasMany(t => t.Messages)
                .WithOne(m => m.Trader)
                .HasForeignKey(m => m.TraderId);
        }
    }
}