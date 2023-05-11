using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using CatalogService.Application.Handlers.Products.v1.Commands;
using CatalogService.Application.Handlers.Products.v1.Requests;
using CatalogService.Test.MockBuilder;
using Xunit;

namespace CatalogService.Test.Commands.Products.v1;

public class DeleteProductTests
{
    [Fact]
    public async Task DeleteProductTest()
    {
        var classToHandle = new DeleteProduct
        {
            Id = ProductMockBuilder.GenerateMockProduct().Id
        };

        
        var handler = (DeleteProductHandler)ProductMockBuilder.CreateHandler<DeleteProductHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }

    [Fact]
    public void DeleteProductInvalidProductIdTest()
    {
        var classToHandle = new DeleteProduct
        {
            Id = null
        };

        var handler = (DeleteProductHandler)ProductMockBuilder.CreateHandler<DeleteProductHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ValidationException>().WithMessage("*Product Id*");
    }
}
