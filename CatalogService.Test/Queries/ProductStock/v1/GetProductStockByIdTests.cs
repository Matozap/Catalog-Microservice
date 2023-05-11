using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using CatalogService.Application.Handlers.ProductStock.v1.Queries;
using CatalogService.Application.Handlers.ProductStock.v1.Requests;
using CatalogService.Test.MockBuilder;
using Xunit;

namespace CatalogService.Test.Queries.ProductStock.v1;

public class GetProductStockByIdTests
{
    [Fact]
    public async Task GetProductStockByIdTestsTest()
    {
        var classToHandle = new GetProductStockById
        {
            Id = ProductStockMockBuilder.GenerateMockProductStock().Id
        };

        var handler = (GetProductStockByIdHandler)ProductStockMockBuilder.CreateHandler<GetProductStockByIdHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }

    [Fact]
    public void GetProductStockByClientIdInvalidRangeTest()
    {
        var classToHandle = new GetProductStockById();
        
        var handler = (GetProductStockByIdHandler)ProductStockMockBuilder.CreateHandler<GetProductStockByIdHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ArgumentNullException>();
    }
}
