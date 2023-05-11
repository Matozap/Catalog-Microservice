using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CatalogService.Domain;
using CatalogService.Infrastructure.Extensions;
using CatalogService.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Database.Context.Seed;

public static class DatabaseSeed
{
    private record SeedProductStock(string Name);
    private record SeedProductImage(string Code, string Name, SeedProductStock[] ProductStock);
    private record SeedProduct(string Code, string Name, string Currency, string CurrencyName, string Region, string SubRegion, SeedProductImage[] ProductImages);

    public static async Task SeedAllProductsDataAsync(DatabaseContext context)
    {
        try
        {
            await context.Database.EnsureCreatedAsync();
            if (context.Products.AsNoTracking().OrderBy(e => e.Id).FirstOrDefault() == null)
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Database","Context","Seed", "seed.json");
                var seedFile = string.Concat(await File.ReadAllLinesAsync(path));
                var productList = seedFile.Deserialize<List<SeedProduct>>();

                context.ChangeTracker.AutoDetectChangesEnabled = false;
                var products = new List<Product>(productList.Count);
                var productImages = new List<ProductImage>();
                var productStock = new List<ProductStock>();
                
                foreach (var product in productList)
                {
                    var newProduct = new Product
                    {
                        Id = UniqueIdGenerator.GenerateSequentialId(),
                        Code = product.Code,
                        Name = product.Name,
                        Currency = product.Currency,
                        CurrencyName = product.CurrencyName,
                        Region = product.Region,
                        SubRegion = product.SubRegion,
                        LastUpdateDate = DateTime.UtcNow,
                        LastUpdateUserId = "System",
                        Disabled = false
                    };
                    products.Add(newProduct);

                    foreach (var productImage in product.ProductImages)
                    {
                        var newProductImage = new ProductImage
                        {
                            Id = UniqueIdGenerator.GenerateSequentialId(),
                            Code = productImage.Code,
                            Name = productImage.Name,
                            ProductId = newProduct.Id,
                            LastUpdateDate = DateTime.UtcNow,
                            LastUpdateUserId = "System",
                            Disabled = false
                        };
                        productImages.Add(newProductImage);

                        productStock.AddRange(productImage.ProductStock.Select(productStock => new ProductStock
                        {
                            Id = UniqueIdGenerator.GenerateSequentialId(),
                            Name = productStock.Name,
                            ProductImageId = newProductImage.Id,
                            LastUpdateDate = DateTime.UtcNow,
                            LastUpdateUserId = "System",
                            Disabled = false
                        }));
                    }
                }

                await context.Products.AddRangeAsync(products);
                await context.ProductImages.AddRangeAsync(productImages);
                await context.ProductStock.AddRangeAsync(productStock);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}  - {ex.StackTrace}");
        }
    }
}
