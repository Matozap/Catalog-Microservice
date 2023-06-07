using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.ProductImages.Commands;
using CatalogService.Application.ProductImages.Requests;
using FluentAssertions;
using CatalogService.Test.MockBuilder;
using Xunit;

namespace CatalogService.Test.Commands.ProductImages.v1;

public class DeleteProductImageTests
{
    [Fact]
    public async Task DeleteProductImageTest()
    {
        var classToHandle = new DeleteProductImage
        {
            Id = ProductImageMockBuilder.GenerateMockProductImage().Id
        };

        var handler = (DeleteProductImageHandler)ProductImageMockBuilder.CreateHandler<DeleteProductImageHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }
}
