using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using CatalogService.Application.Handlers.ProductStock.v1.Queries;
using CatalogService.Message.Contracts.ProductStock.v1.Requests;
using CatalogService.Test.MockBuilder;
using Xunit;

namespace CatalogService.Test.Queries.ProductStock.v1;

public class GetAllProductStockTests
{
    [Fact]
    public async Task GetAllProductStockTest()
    {
        var classToHandle = new GetAllProductStock
        {
            ProductStockId = "1"
        };
        
        var handler = (GetAllProductStockHandler)ProductStockMockBuilder.CreateHandler<GetAllProductStockHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull().And.HaveCountGreaterThan(1);
    }
}
