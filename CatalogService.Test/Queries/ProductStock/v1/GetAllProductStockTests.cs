using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.ProductStock.Queries;
using CatalogService.Application.ProductStock.Requests;
using FluentAssertions;
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
            ProductId = "1"
        };
        
        var handler = (GetAllProductStockHandler)ProductStockMockBuilder.CreateHandler<GetAllProductStockHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull().And.HaveCountGreaterThan(1);
    }
}
