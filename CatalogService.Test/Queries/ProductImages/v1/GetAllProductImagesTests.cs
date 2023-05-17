using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using CatalogService.Application.Handlers.ProductImages.v1.Queries;
using CatalogService.Message.Contracts.ProductImages.v1;
using CatalogService.Message.Contracts.ProductImages.v1.Requests;
using CatalogService.Test.MockBuilder;
using Xunit;

namespace CatalogService.Test.Queries.ProductImages.v1;

public class GetAllProductImagesTests
{
    [Fact]
    public async Task GetAllProductImagesTest()
    {
        var classToHandle = new GetAllProductImages
        {
            ProductId = "CO"
        };
        
        var handler = (GetAllProductImagesHandler)ProductImageMockBuilder.CreateHandler<GetAllProductImagesHandler>();
        var result = (List<ProductImageData>)await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull().And.HaveCountGreaterThan(1);
    }
}
