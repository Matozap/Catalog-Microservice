using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using CatalogService.Application.Handlers.ProductStock.v1.Commands;
using CatalogService.Application.Handlers.ProductStock.v1.Requests;
using CatalogService.Message.Contracts.ProductStock.v1;
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
            Id = "1",
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
            Id = "1",
            Value = 10
        };

        var handler = (UpdateProductStockHandler)ProductStockMockBuilder.CreateHandler<UpdateProductStockHandler>();

        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ArgumentNullException>().WithMessage("*Id*");
    }
}
