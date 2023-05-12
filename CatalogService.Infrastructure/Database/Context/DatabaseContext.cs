using System;
using CatalogService.Domain;
using CatalogService.Infrastructure.Database.Context.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CatalogService.Infrastructure.Database.Context;

public sealed class DatabaseContext : DbContext
{
    private readonly ILogger<DatabaseContext> _logger;
    private readonly DatabaseOptions _databaseOptions;

    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options, ILogger<DatabaseContext> logger, DatabaseOptions databaseOptions)
        : base(options)
    {
        _logger = logger;
        _databaseOptions = databaseOptions;
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<ProductStock> ProductStock { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Outbox> EventBusOutboxes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _logger.LogInformation("Using {DatabaseType} database ({DatabaseEngine})", _databaseOptions.DatabaseType, Enum.GetName(typeof(EngineType), _databaseOptions.EngineType));
        CreateProductModel(modelBuilder);
        CreateProductImageModel(modelBuilder);
        CreateProductStockModel(modelBuilder);
        CreateEventBusOutboxModel(modelBuilder);
        CreateProductCategoryModel(modelBuilder);
    }
    
    private static void CreateProductModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().ToContainer("Product");
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ToJsonProperty("id").IsRequired();
            entity.Property(e => e.Sku).IsRequired();
            entity.HasIndex(e => e.Sku);
            entity.Property(e => e.Name).IsRequired();
            entity.HasIndex(e => e.Sku);
            entity.HasMany(c => c.ProductImages);
            entity.HasNoDiscriminator();
            entity.HasPartitionKey(e => e.Id);
        });
    }

    private static void CreateProductImageModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductImage>().ToContainer("ProductImage");
        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ToJsonProperty("id").IsRequired();
            entity.Property(e => e.Code).IsRequired();
            entity.HasIndex(e => e.Code);
            entity.Property(e => e.Name).IsRequired();
            entity.HasIndex(e => e.Name);
            entity.HasMany(s => s.ProductStock).WithOne(c => c.ProductImage);
            entity.HasNoDiscriminator();
            entity.HasPartitionKey(e => e.ProductId);
            entity.HasManualThroughput(10000);
        });
    }

    private static void CreateProductStockModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductStock>().ToContainer("ProductStock");
        modelBuilder.Entity<ProductStock>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ToJsonProperty("id").IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.HasIndex(e => e.Name);
            entity.HasOne(productStock => productStock.ProductImage).WithMany(productImage => productImage.ProductStock);
            entity.HasNoDiscriminator();
            entity.HasPartitionKey(e => e.Id);
            entity.HasManualThroughput(10000);
        });
    }
    
    private static void CreateProductCategoryModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductCategory>().ToContainer("ProductCategory");
        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ToJsonProperty("id").IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.HasIndex(e => e.Name);
            entity.HasMany(s => s.Products).WithOne(c => c.ProductCategory);
            entity.HasNoDiscriminator();
            entity.HasPartitionKey(e => e.Id);
            entity.HasManualThroughput(10000);
        });
    }
    
    private static void CreateEventBusOutboxModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Outbox>().ToContainer("EventBusOutbox");
        modelBuilder.Entity<Outbox>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ToJsonProperty("id").IsRequired();
            entity.Property(e => e.JsonObject).IsRequired();
            entity.Property(e => e.Operation).IsRequired();
            entity.HasIndex(e => e.Operation);
            entity.HasNoDiscriminator();
            entity.HasPartitionKey(e => e.Id);
        });
    }

    public void SeedData() => DatabaseSeed.SeedAllProductsDataAsync(this).Wait();
}
