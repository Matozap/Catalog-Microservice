using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using CatalogService.Application.Handlers.ProductStock.v1.Commands;
using CatalogService.Message.Contracts.ProductStock.v1;
using CatalogService.Message.Contracts.ProductStock.v1.Requests;
using CatalogService.Test.MockBuilder;
using Xunit;

namespace CatalogService.Test.Commands.ProductStock.v1;

public class CreateProductStockTests
{
    [Fact]
    public async Task CreateProductStockTest()
    {
        var classToHandle = new CreateProductStock
        {
            Details = ProductStockMockBuilder.GenerateMockProductStockDtoList(1).First()
        };

        var handler = (CreateProductStockHandler)ProductStockMockBuilder.CreateHandler<CreateProductStockHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull().And.BeOfType<ProductStockData>();
    }

    [Fact]
    public void CreateProductStockInvalidNameTest()
    {
        var resultDto = ProductStockMockBuilder.GenerateMockProductStockDtoList(1).First();
        resultDto.Current = 1;
        
        var classToHandle = new CreateProductStock
        {
            Details = resultDto
        };
        
        var handler = (CreateProductStockHandler)ProductStockMockBuilder.CreateHandler<CreateProductStockHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());
    
        action.Should().ThrowAsync<ValidationException>().WithMessage("*Name*");
    }
}
