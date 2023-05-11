using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoFixture;
using DistributedCache.Core;
using CatalogService.Application.Handlers.ProductStock.v1.Commands;
using CatalogService.Application.Handlers.ProductStock.v1.Queries;
using CatalogService.Application.Handlers.ProductStock.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using CatalogService.Message.Contracts.ProductStock.v1;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace CatalogService.Test.MockBuilder;

public static class ProductStockMockBuilder
{
    private static readonly Fixture Fixture = new();

    private static IRepository GenerateMockRepository(ProductStock catalog = null, int rowCount = 100)
    {
        var mockProductStock = catalog ?? GenerateMockProductStock();
        var mockProductStockList = GenerateMockDomainProductStockList(rowCount);
        
        var repository = Substitute.For<IRepository>();
        
        repository.AddAsync(Arg.Any<ProductStock>()).Returns(mockProductStock);
        repository.UpdateAsync(mockProductStock).Returns(mockProductStock);
        repository.DeleteAsync(mockProductStock).Returns(mockProductStock);
        
        repository.GetAsSingleAsync(Arg.Any<Expression<Func<ProductStock, bool>>>(), Arg.Any<Expression<Func<ProductStock, string>>>(), 
            Arg.Any<Expression<Func<ProductStock, string>>>(), Arg.Any<Expression<Func<ProductStock, ProductStock>>>(), Arg.Any<bool>()).Returns(mockProductStock);
        repository.GetAsListAsync(Arg.Any<Expression<Func<ProductStock, bool>>>(), Arg.Any<Expression<Func<ProductStock, string>>>(), 
            Arg.Any<Expression<Func<ProductStock, string>>>(), Arg.Any<Expression<Func<ProductStock, ProductStock>>>(), Arg.Any<bool>()).Returns(mockProductStockList);
        return repository;
    }

    private static ICache GenerateMockObjectCache()
    {
        var cache = Substitute.For<ICache>();
        return cache;
    }

    private static List<ProductStock> GenerateMockDomainProductStockList(int count)
    {
        return Fixture.Build<ProductStock>()
            .With(s => s.ProductImage, () => new ProductImage())
            .With(s => s.LastUpdateDate, () => DateTime.Now)
            .With(s => s.LastUpdateUserId, () => "Test")
            .CreateMany(count)
            .ToList();
    }

    public static List<ProductStockData> GenerateMockProductStockDtoList(int count)
    {
        return Fixture.Build<ProductStockData>()
            .CreateMany(count).ToList();
    }

    public static ProductStock GenerateMockProductStock()
    {
        return Fixture.Build<ProductStock>()
            .Without(s => s.ProductImage)
            .With(s => s.LastUpdateDate, () => DateTime.Now)
            .With(s => s.LastUpdateUserId, () => "Test")
            .Create();
    }


    public static object CreateHandler<T>()
    {
        var response = GenerateMockProductStockDtoList(1).FirstOrDefault();
        var catalog = GenerateMockProductStock();
        
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetProductStockById>()).Returns(response);

        if (typeof(T) == typeof(UpdateProductStockHandler))
        {
            return new UpdateProductStockHandler(NullLogger<UpdateProductStockHandler>.Instance,
                GenerateMockRepository(catalog));
        }
        
        if (typeof(T) == typeof(SoftDeleteProductStockHandler))
        {
            return new SoftDeleteProductStockHandler(NullLogger<SoftDeleteProductStockHandler>.Instance,
                GenerateMockRepository(catalog));
        }
        
        if (typeof(T) == typeof(DeleteProductStockHandler))
        {
            return new DeleteProductStockHandler(NullLogger<DeleteProductStockHandler>.Instance,
                GenerateMockRepository(catalog));
        }
        
        if (typeof(T) == typeof(CreateProductStockHandler))
        {
            var repository = GenerateMockRepository(catalog);
            repository.GetAsSingleAsync(Arg.Any<Expression<Func<ProductStock, bool>>>(), Arg.Any<Expression<Func<ProductStock, string>>>(), 
                Arg.Any<Expression<Func<ProductStock, string>>>(), Arg.Any<Expression<Func<ProductStock, ProductStock>>>(), Arg.Any<bool>()).Returns((ProductStock)null);
            return new CreateProductStockHandler(NullLogger<CreateProductStockHandler>.Instance,
                repository);
        }
        
        if (typeof(T) == typeof(GetAllProductStockHandler))
        {
            return new GetAllProductStockHandler(GenerateMockObjectCache(),
                NullLogger<GetAllProductStockHandler>.Instance,
                GenerateMockRepository(rowCount: 10));
        }
        
        if (typeof(T) == typeof(GetProductStockByIdHandler))
        {
            return new GetProductStockByIdHandler(GenerateMockRepository(catalog),
                GenerateMockObjectCache(),
                NullLogger<GetProductStockByIdHandler>.Instance);
        }
        
        return null;
    } 
}
