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
    private record SeedProductCategory(string Name, string Description, SeedProduct[] Products);
    private record SeedProduct(string Sku, string Name, string Description, decimal Price, string Brand,string Dimensions, decimal Weight);

    public static async Task SeedAllProductsDataAsync(DatabaseContext context)
    {
        try
        {
            await context.Database.EnsureCreatedAsync();
            if (context.Products.AsNoTracking().OrderBy(e => e.Id).FirstOrDefault() == null)
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Database","Context","Seed", "seed.json");
                var seedFile = string.Concat(await File.ReadAllLinesAsync(path));
                var productCategoryList = seedFile.Deserialize<List<SeedProductCategory>>();

                context.ChangeTracker.AutoDetectChangesEnabled = false;
                var productCategories = new List<ProductCategory>(productCategoryList.Count);
                var products = new List<Product>();
                var productImages = new List<ProductImage>();
                var productStock = new List<ProductStock>();

                foreach (var category in productCategoryList)
                {
                    var productCategory = new ProductCategory
                    {
                        Id = UniqueIdGenerator.GenerateSequentialId(),
                        Name = category.Name,
                        Description = category.Description,
                        Disabled = false,
                        LastUpdateDate = DateTime.UtcNow,
                        LastUpdateUserId = "System"
                    };

                    productCategories.Add(productCategory);

                    foreach (var product in category.Products)
                    {
                        var newProduct = new Product
                        {
                            Id = UniqueIdGenerator.GenerateSequentialId(),
                            Sku = product.Sku,
                            Name = product.Name,
                            Description = product.Description,
                            Price = product.Price,
                            ProductCategoryId = productCategory.Id,
                            Brand = product.Brand,
                            Dimensions = product.Dimensions,
                            Weight = product.Weight,
                            LastUpdateDate = DateTime.UtcNow,
                            LastUpdateUserId = "System",
                            Disabled = false
                        };
                        products.Add(newProduct);

                        for (int i = 0; i < 2; i++)
                        {
                            var newProductImage = new ProductImage
                            {
                                Id = UniqueIdGenerator.GenerateSequentialId(),
                                Title = $"Image {newProduct.Name}",
                                Url = $"http://matozap.com/images/{i}",
                                ProductId = newProduct.Id,
                                LastUpdateDate = DateTime.UtcNow,
                                LastUpdateUserId = "System",
                                Disabled = false
                            };
                            productImages.Add(newProductImage);
                        }
                        
                        productStock.Add(new ProductStock
                        {
                            Id = UniqueIdGenerator.GenerateSequentialId(),
                            Current = 0,
                            Booked = 0,
                            Previous = 0,
                            ProductId = newProduct.Id,
                            LastUpdateDate = DateTime.UtcNow,
                            LastUpdateUserId = "System"
                        });
                    }

                }

                await context.ProductCategories.AddRangeAsync(productCategories);
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
