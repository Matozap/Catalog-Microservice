using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using CatalogService.Application.Handlers.ProductImages.v1.Commands;
using CatalogService.Application.Handlers.ProductImages.v1.Requests;
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
