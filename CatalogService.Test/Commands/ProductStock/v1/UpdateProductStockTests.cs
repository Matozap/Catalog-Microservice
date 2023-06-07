using System;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.ProductStock.Commands;
using CatalogService.Application.ProductStock.Requests;
using FluentAssertions;
using CatalogService.Test.MockBuilder;
using Xunit;

namespace CatalogService.Test.Commands.ProductStock.v1;

public class UpdateProductStockTests
{
    [Fact]
    public async Task UpdateProductStockTest()
    {
        var classToHandle = new UpdateProductStock
        {
            ProductId = "1",
            Value = 10
        };

        var handler = (UpdateProductStockHandler)ProductStockMockBuilder.CreateHandler<UpdateProductStockHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }

    [Fact]
    public void UpdateProductStockInvalidProductStockIdTest()
    {
        var classToHandle = new UpdateProductStock
        {
            ProductId = "1",
            Value = 10
        };

        var handler = (UpdateProductStockHandler)ProductStockMockBuilder.CreateHandler<UpdateProductStockHandler>();

        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ArgumentNullException>().WithMessage("*Id*");
    }
}
