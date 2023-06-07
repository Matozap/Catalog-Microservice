using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.ProductImages.Commands;
using CatalogService.Application.ProductImages.Requests;
using FluentAssertions;
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
