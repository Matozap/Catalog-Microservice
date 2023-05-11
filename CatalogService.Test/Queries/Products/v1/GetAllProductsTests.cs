using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using CatalogService.Application.Handlers.Products.v1.Queries;
using CatalogService.Application.Handlers.Products.v1.Requests;
using CatalogService.Test.MockBuilder;
using Xunit;

namespace CatalogService.Test.Queries.Products.v1;

public class GetAllProductsTests
{
    [Fact]
    public async Task GetAllProductsTest()
    {
        var classToHandle = new GetAllProducts();
        
        var handler = (GetAllProductsHandler)ProductMockBuilder.CreateHandler<GetAllProductsHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull().And.HaveCountGreaterThan(1);
    }
}
