using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using CatalogService.Application.Handlers.ProductStock.v1.Commands;
using CatalogService.Application.Handlers.ProductStock.v1.Requests;
using CatalogService.Test.MockBuilder;
using Xunit;

namespace CatalogService.Test.Commands.ProductStock.v1;

public class SoftDeleteProductStockTests
{
    [Fact]
    public async Task SoftDeleteProductStockTest()
    {
        var classToHandle = new SoftDeleteProductStock
        {
            Id = ProductStockMockBuilder.GenerateMockProductStock().Id
        };
        
        var handler = (SoftDeleteProductStockHandler)ProductStockMockBuilder.CreateHandler<SoftDeleteProductStockHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }
}
