using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using Bustr.Bus;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Products.Commands;
using CatalogService.Application.Products.Events;
using CatalogService.Application.Products.Queries;
using CatalogService.Application.Products.Requests;
using CatalogService.Application.Products.Responses;
using DistributedCache.Core;
using CatalogService.Domain;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace CatalogService.Test.MockBuilder;

public static class ProductMockBuilder
{
    private static readonly Fixture Fixture = new();

    private static IRepository GenerateMockRepository(Product catalog = null, int rowCount = 100)
    {
        var mockProduct = catalog ?? GenerateMockProduct();
        var mockCounties = GenerateMockDomainProductList(rowCount);
        
        var repository = Substitute.For<IRepository>();
        
        repository.AddAsync(Arg.Any<Product>()).Returns(mockProduct);
        repository.UpdateAsync(mockProduct).Returns(mockProduct);
        repository.DeleteAsync(mockProduct).Returns(mockProduct);
        
        repository.GetAsSingleAsync(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<Expression<Func<Product, string>>>(), 
            Arg.Any<Expression<Func<Product, string>>>(), Arg.Any<Expression<Func<Product, Product>>>(), Arg.Any<bool>()).Returns(mockProduct);
        repository.GetAsListAsync(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<Expression<Func<Product, string>>>(), 
            Arg.Any<Expression<Func<Product, string>>>(), Arg.Any<Expression<Func<Product, Product>>>(), Arg.Any<bool>()).Returns(mockCounties);
        return repository;
    }

    private static ICache GenerateMockObjectCache()
    {
        var cache = Substitute.For<ICache>();
        return cache;
    }
    
    private static IEventBus GenerateMockEventBus()
    {
        var eventBus = Substitute.For<IEventBus>();
        eventBus.PublishAsync(Arg.Any<ProductEvent>()).Returns(Task.CompletedTask);
        return eventBus;
    }

    private static List<Product> GenerateMockDomainProductList(int count)
    {
        return Fixture.Build<Product>()
            .Without(s => s.ProductImages)
            .Without(s => s.ProductStocks)
            .Without(s => s.ProductCategory)
            .With(s => s.LastUpdateDate, () => DateTime.Now)
            .With(s => s.LastUpdateUserId, () => "Test")
            .CreateMany(count)
            .ToList();
    }

    public static List<ProductData> GenerateMockProductDtoList(int count)
    {
        return Fixture.Build<ProductData>()
            .Without(s => s.ProductImages)
            .CreateMany(count).ToList();
    }

    public static Product GenerateMockProduct()
    {
        return Fixture.Build<Product>()
            .With(s => s.LastUpdateDate, () => DateTime.Now)
            .With(s => s.LastUpdateUserId, () => "Test")
            .Without(s => s.ProductImages)
            .Without(s => s.ProductStocks)
            .Without(s => s.ProductCategory)
            .Create();
    }


    public static object CreateHandler<T>()
    {
        var response = GenerateMockProductDtoList(1).FirstOrDefault();
        var catalog = GenerateMockProduct();
        
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetProductById>()).Returns(response);

        if (typeof(T) == typeof(UpdateProductHandler))
        {
            return new UpdateProductHandler(NullLogger<UpdateProductHandler>.Instance,
                GenerateMockRepository(catalog), GenerateMockObjectCache(), GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(SoftDeleteProductHandler))
        {
            return new SoftDeleteProductHandler(NullLogger<SoftDeleteProductHandler>.Instance,
                GenerateMockRepository(catalog), GenerateMockObjectCache(), GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(DeleteProductHandler))
        {
            return new DeleteProductHandler(NullLogger<DeleteProductHandler>.Instance,
                GenerateMockRepository(catalog), GenerateMockObjectCache(), GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(CreateProductHandler))
        {
            var repository = GenerateMockRepository(catalog);
            repository.GetAsSingleAsync(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<Expression<Func<Product, string>>>(), 
                Arg.Any<Expression<Func<Product, string>>>(), Arg.Any<Expression<Func<Product, Product>>>(), Arg.Any<bool>()).Returns((Product)null);
            return new CreateProductHandler(NullLogger<CreateProductHandler>.Instance,
                repository, GenerateMockObjectCache(), GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(GetAllProductsHandler))
        {
            return new GetAllProductsHandler(GenerateMockObjectCache(),
                NullLogger<GetAllProductsHandler>.Instance,
                GenerateMockRepository(rowCount: 10));
        }
        
        if (typeof(T) == typeof(GetProductByIdHandler))
        {
            return new GetProductByIdHandler(GenerateMockRepository(catalog),
                GenerateMockObjectCache(),
                NullLogger<GetProductByIdHandler>.Instance);
        }
        
        return null;
    } 
}
