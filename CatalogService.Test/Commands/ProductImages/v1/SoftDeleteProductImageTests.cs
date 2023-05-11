using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using CatalogService.Application.Handlers.ProductImages.v1.Commands;
using CatalogService.Application.Handlers.ProductImages.v1.Requests;
using CatalogService.Test.MockBuilder;
using Xunit;

namespace CatalogService.Test.Commands.ProductImages.v1;

public class SoftDeleteProductImageTests
{
    [Fact]
    public async Task SoftDeleteProductImageTest()
    {
        var classToHandle = new SoftDeleteProductImage
        {
            Id = ProductImageMockBuilder.GenerateMockProductImage().Id
        };
        
        var handler = (SoftDeleteProductImageHandler)ProductImageMockBuilder.CreateHandler<SoftDeleteProductImageHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }
}
