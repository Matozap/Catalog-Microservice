using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentValidation;
using CatalogService.Application.Events.Publishers;
using CatalogService.Application.Pipeline;
using CatalogService.Domain;
using CatalogService.Message.Contracts.ProductStock.v1;
using CatalogService.Message.Contracts.Products.v1;
using CatalogService.Message.Contracts.ProductImages.v1;
using Mapster;
using MediatR;
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
        TypeAdapterConfig<ProductImage, ProductImageData>
            .NewConfig()
            .Map(dest => dest.ProductStock, src => MapToProductStockData(src))
            .IgnoreNullValues(true);
        TypeAdapterConfig<ProductImageData, ProductImage>
            .NewConfig()
            .Map(dest => dest.Product, src => (Product)null)
            .IgnoreNullValues(true);
        TypeAdapterConfig<ProductStock, ProductStockData>
            .NewConfig()
            .IgnoreNullValues(true);
        TypeAdapterConfig<ProductStockData, ProductStock>
            .NewConfig()
            .Map(dest => dest.ProductImage, src => (ProductImage)null)
            .IgnoreNullValues(true);

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IOutboxPublisher, OutboxPublisher>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ApplicationPipelineBehaviour<,>));
        return services;
    }

    private static IEnumerable<ProductImageData> MapToProductImageData(Product src)
    {
        return src.ProductImages.Select(productImage => new ProductImageData
        {
            Id = productImage.Id,
            Code = productImage.Code,
            Name = productImage.Name,
            ProductId = productImage.ProductId,
            ProductStock =  MapToProductStockData(productImage).ToList()
        });
    }

    private static IEnumerable<ProductStockData> MapToProductStockData(ProductImage src)
    {
        return src.ProductStock.Select(productStock => new ProductStockData
        {
            Id = productStock.Id,
            Name = productStock.Name,
            ProductImageId = productStock.ProductImageId
        });
    }

    public static IApplicationBuilder UseApplication(this IApplicationBuilder app)
    {
        return app;
    }
}
