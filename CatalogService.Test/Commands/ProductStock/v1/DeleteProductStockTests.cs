using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using CatalogService.Application.Handlers.ProductStock.v1.Commands;
using CatalogService.Application.Handlers.ProductStock.v1.Requests;
using CatalogService.Test.MockBuilder;
using Xunit;

namespace CatalogService.Test.Commands.ProductStock.v1;

public class DeleteProductStockTests
{
    [Fact]
    public async Task DeleteProductStockTest()
    {
        var classToHandle = new DeleteProductStock
        {
            Id = ProductStockMockBuilder.GenerateMockProductStock().Id
        };

        var handler = (DeleteProductStockHandler)ProductStockMockBuilder.CreateHandler<DeleteProductStockHandler>();

        var result = await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }
}
