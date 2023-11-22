using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using Bustr.Bus;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.ProductStock.Commands;
using CatalogService.Application.ProductStock.Events;
using CatalogService.Application.ProductStock.Queries;
using CatalogService.Application.ProductStock.Requests;
using CatalogService.Application.ProductStock.Responses;
using CatalogService.Domain;
using Distributed.Cache.Core;
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
    
    private static IEventBus GenerateMockEventBus()
    {
        var eventBus = Substitute.For<IEventBus>();
        eventBus.PublishAsync(Arg.Any<ProductStockEvent>()).Returns(Task.CompletedTask);
        eventBus.PublishAsync(Arg.Any<ProductStockBookEvent>()).Returns(Task.CompletedTask);
        eventBus.PublishAsync(Arg.Any<ProductStockReleaseEvent>()).Returns(Task.CompletedTask);
        return eventBus;
    }

    private static List<ProductStock> GenerateMockDomainProductStockList(int count)
    {
        return Fixture.Build<ProductStock>()
            .With(s => s.LastUpdateDate, () => DateTime.Now)
            .With(s => s.LastUpdateUserId, () => "Test")
            .Without(s => s.Product)
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
            .With(s => s.LastUpdateDate, () => DateTime.Now)
            .With(s => s.LastUpdateUserId, () => "Test")
            .Without(s => s.Product)
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
                GenerateMockRepository(catalog), GenerateMockObjectCache(), GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(DeleteProductStockHandler))
        {
            return new DeleteProductStockHandler(NullLogger<DeleteProductStockHandler>.Instance,
                GenerateMockRepository(catalog), GenerateMockObjectCache(), GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(CreateProductStockHandler))
        {
            var repository = GenerateMockRepository(catalog);
            repository.GetAsSingleAsync(Arg.Any<Expression<Func<ProductStock, bool>>>(), Arg.Any<Expression<Func<ProductStock, string>>>(), 
                Arg.Any<Expression<Func<ProductStock, string>>>(), Arg.Any<Expression<Func<ProductStock, ProductStock>>>(), Arg.Any<bool>()).Returns((ProductStock)null);
            return new CreateProductStockHandler(NullLogger<CreateProductStockHandler>.Instance,
                repository, GenerateMockObjectCache(), GenerateMockEventBus());
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
