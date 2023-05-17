using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using CatalogService.Application.Handlers.Products.v1.Queries;
using CatalogService.Message.Contracts.Products.v1.Requests;
using CatalogService.Test.MockBuilder;
using Xunit;

namespace CatalogService.Test.Queries.Products.v1;

public class GetProductByIdTests
{
    [Fact]
    public async Task GetProductByIdTestsTest()
    {
        var classToHandle = new GetProductById
        {
            Id = ProductMockBuilder.GenerateMockProduct().Id
        };

        var handler = (GetProductByIdHandler)ProductMockBuilder.CreateHandler<GetProductByIdHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }

    [Fact]
    public void GetProductByClientIdInvalidRangeTest()
    {
        var classToHandle = new GetProductById();
        
        var handler = (GetProductByIdHandler)ProductMockBuilder.CreateHandler<GetProductByIdHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ArgumentNullException>();
    }
}
