using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using CatalogService.Application.Handlers.ProductImages.v1.Commands;
using CatalogService.Application.Handlers.ProductImages.v1.Requests;
using CatalogService.Message.Contracts.ProductImages.v1;
using CatalogService.Test.MockBuilder;
using Xunit;

namespace CatalogService.Test.Commands.ProductImages.v1;

public class UpdateProductImageTests
{
    [Fact]
    public async Task UpdateProductImageTest()
    {
        var classToHandle = new UpdateProductImage
        {
            Details = ProductImageMockBuilder.GenerateMockProductImageDtoList(1).FirstOrDefault()
        };

        var handler = (UpdateProductImageHandler)ProductImageMockBuilder.CreateHandler<UpdateProductImageHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }

    [Fact]
    public void UpdateProductImageInvalidProductImageIdTest()
    {
        var classToHandle = new UpdateProductImage
        {
            Details = new ProductImageData()
        };

        var handler = (UpdateProductImageHandler)ProductImageMockBuilder.CreateHandler<UpdateProductImageHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ArgumentNullException>().WithMessage("*Id*");
    }
}
