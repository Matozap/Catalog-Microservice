using System;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.ProductImages.Queries;
using CatalogService.Application.ProductImages.Requests;
using CatalogService.Application.ProductImages.Responses;
using FluentAssertions;
using CatalogService.Test.MockBuilder;
using Xunit;

namespace CatalogService.Test.Queries.ProductImages.v1;

public class GetProductImageByIdTests
{
    [Fact]
    public async Task GetProductImageByIdTestsTest()
    {
        var classToHandle = new GetProductImageById
        {
            Id = ProductImageMockBuilder.GenerateMockProductImage().Id
        };

        var handler = (GetProductImageByIdHandler)ProductImageMockBuilder.CreateHandler<GetProductImageByIdHandler>();
        var result = (ProductImageData)await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }

    [Fact]
    public void GetProductImageByClientIdInvalidRangeTest()
    {
        var classToHandle = new GetProductImageById();
        
        var handler = (GetProductImageByIdHandler)ProductImageMockBuilder.CreateHandler<GetProductImageByIdHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ArgumentNullException>();
    }
}
