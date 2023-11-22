using System;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bustr;
using CatalogService.API.Endpoints.Consumers.Self;
using CatalogService.API.Endpoints.Grpc;
using CatalogService.API.Helpers.Configuration;
using CatalogService.API.Helpers.Middleware;
using CatalogService.Application;
using CatalogService.Application.ProductCategories.Events;
using CatalogService.Application.ProductImages.Events;
using CatalogService.Application.Products.Events;
using CatalogService.Application.ProductStock.Events;
using CatalogService.Infrastructure;
using Distributed.Cache.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.Server;
using Serilog;

namespace CatalogService.API.Helpers;

public static class StartupServicesExtension
{
    public static void AddWebApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors();
        services.AddHealthChecks();
        services.AddControllers()
            .AddJsonOptions(x => x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
        services.AddMvc();
        services.ConfigureSwagger();
        services.AddApplicationInsightsTelemetry();
        services.AddResponseCompression();
        services.Configure<GzipCompressionProviderOptions>
        (options => 
        { 
            options.Level = CompressionLevel.Fastest; 
        }); 
        
        AddRequiredServices(services, configuration);
        AddOptionalServices(services, configuration);
        services.AddTransient<GlobalExceptionMiddleware>();
    }
    
    public static void AddFunctionServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.Configure<JsonSerializerOptions>(options =>
        {
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.PropertyNameCaseInsensitive = true;
        });
        
        AddRequiredServices(services, configuration);
    }
    
    private static void AddOptionalServices(this IServiceCollection services, IConfiguration configuration)
    {
        var grpcSettings = ConfigurationHelper.GrpcSettings(configuration);
        if(grpcSettings.Disabled) return;
        services.AddCodeFirstGrpc(
            config =>
            {
                config.ResponseCompressionLevel = CompressionLevel.Optimal;
                config.EnableDetailedErrors = true;
                config.IgnoreUnknownServices = true;
                config.Interceptors.Add<GlobalExceptionMiddleware>();
            }
        );
        if(grpcSettings.ReflectionDisabled) return;
        services.AddCodeFirstGrpcReflection();
    }
    
    private static void AddRequiredServices(IServiceCollection services, IConfiguration configuration)
    {
        var busSettings = ConfigurationHelper.BusSettings(configuration);
        var cacheSettings = ConfigurationHelper.CacheSettings(configuration);
        
        services.AddApplication()
            .AddInfrastructure(configuration)
            .AddDistributedCache(options =>
            {
                options.Configure(cacheSettings.CacheType, cacheSettings.ConnectionString, cacheSettings.InstanceName)
                    .ConfigureHealthCheck(true, 5, 2)
                    .DisableCache(cacheSettings.Disabled);
            })
            .AddBustr(options =>
            {
                options.Configure(busSettings.BusType, busSettings.ConnectionString)
                    .SetRetryIntervals(TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10))
                    .UseDeadLetterQueue(true)
                    .DisableBus(busSettings.Disabled)
                    .MapTopic("catalog/product-category-event", typeof(ProductCategoryEvent), typeof(ProductCategoryEventConsumer), "self.product.category.catalog.sub")
                    .MapTopic("catalog/product-event", typeof(ProductEvent), typeof(ProductEventConsumer), "self.product.catalog.sub")
                    .MapTopic("catalog/product-image-event", typeof(ProductImageEvent), typeof(ProductImageEventConsumer), "self.product.image.catalog.sub")
                    .MapTopic("catalog/product-stock-event", typeof(ProductStockEvent), typeof(ProductStockEventConsumer), "self.product.stock.catalog.sub")
                    .MapTopic("catalog/product-stock-book-event", typeof(ProductStockBookEvent))
                    .MapTopic("catalog/product-stock-release-event", typeof(ProductStockReleaseEvent));
            });
    }
    
    public static void AddHostMiddleware(this WebApplication app, IWebHostEnvironment environment, IConfiguration configuration)
    {
        app.UseSwaggerApi();
        app.UseApplication()
            .UseInfrastructure(environment)
            .UseRouting();
        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
        app.MapHealthChecks("/heartbeat");
        app.MapControllers();
        app.UseMiddleware<GlobalExceptionMiddleware>();
        
        if(ConfigurationHelper.GrpcSettings(configuration).Disabled) return;
        app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true});
        app.MapGrpcService<ProductStockService>();
        app.MapGrpcService<ProductImageService>();
        app.MapGrpcService<ProductService>();
        app.MapGrpcService<ProductCategoryService>();
        
        if(ConfigurationHelper.GrpcSettings(configuration).ReflectionDisabled) return;
        app.MapCodeFirstGrpcReflectionService();
    }

    public static IHostBuilder CreateLogger(this IHostBuilder webHostBuilder)
    {
        return webHostBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name ?? "Application")
                .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment);
        });
    }
}
