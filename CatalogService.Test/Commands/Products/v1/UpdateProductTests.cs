using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using CatalogService.Application.Handlers.Products.v1.Commands;
using CatalogService.Message.Contracts.Products.v1;
using CatalogService.Message.Contracts.Products.v1.Requests;
using CatalogService.Test.MockBuilder;
using Xunit;

namespace CatalogService.Test.Commands.Products.v1;

public class UpdateProductTests
{
    [Fact]
    public async Task UpdateProductTest()
    {
        var classToHandle = new UpdateProduct
        {
            Details = ProductMockBuilder.GenerateMockProductDtoList(1).FirstOrDefault()
        };

        var handler = (UpdateProductHandler)ProductMockBuilder.CreateHandler<UpdateProductHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }

    [Fact]
    public void UpdateProductInvalidProductIdTest()
    {
        var classToHandle = new UpdateProduct
        {
            Details = new ProductData()
        };

        var handler = (UpdateProductHandler)ProductMockBuilder.CreateHandler<UpdateProductHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ArgumentNullException>().WithMessage("*Id*");
    }
}
