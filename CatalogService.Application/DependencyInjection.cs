using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CatalogService.Application.ProductImages.Responses;
using CatalogService.Application.Products.Responses;
using CatalogService.Application.ProductStock.Responses;
using CatalogService.Domain;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogService.Application;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        TypeAdapterConfig<Product, ProductData>
            .NewConfig()
            .Map(dest => dest.ProductImages, src => MapToProductImageData(src), src => src.ProductImages != null)
            .TwoWays()
            .IgnoreNullValues(true);
        TypeAdapterConfig<ProductData, Product>
            .NewConfig()
            .Map(dest => dest.ProductCategory, src => (ProductCategory)null)
            .TwoWays()
            .IgnoreNullValues(true);
        TypeAdapterConfig<ProductImage, ProductImageData>
            .NewConfig()
            .IgnoreNullValues(true);
        TypeAdapterConfig<ProductImageData, ProductImage>
            .NewConfig()
            .Map(dest => dest.Product, src => (Product)null)
            .IgnoreNullValues(true);
        TypeAdapterConfig<Domain.ProductStock, ProductStockData>
            .NewConfig()
            .IgnoreNullValues(true);
        TypeAdapterConfig<ProductStockData, Domain.ProductStock>
            .NewConfig()
            .IgnoreNullValues(true);

        return services;
    }
    
    private static IEnumerable<ProductImageData> MapToProductImageData(Product src)
    {
        return src.ProductImages.Select(productImage => new ProductImageData
        {
            Id = productImage.Id,
            Title = productImage.Title,
            Url = productImage.Url,
            ProductId = productImage.ProductId
        });
    }

    public static IApplicationBuilder UseApplication(this IApplicationBuilder app)
    {
        return app;
    }
}
