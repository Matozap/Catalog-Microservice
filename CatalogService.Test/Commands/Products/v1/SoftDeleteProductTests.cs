using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Products.Commands;
using CatalogService.Application.Products.Requests;
using FluentAssertions;
using CatalogService.Test.MockBuilder;
using Xunit;

namespace CatalogService.Test.Commands.Products.v1;

public class SoftDeleteProductTests
{
    [Fact]
    public async Task SoftDeleteProductTest()
    {
        var classToHandle = new SoftDeleteProduct
        {
            Id = ProductMockBuilder.GenerateMockProduct().Id
        };
        
        var handler = (SoftDeleteProductHandler)ProductMockBuilder.CreateHandler<SoftDeleteProductHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }

    [Fact]
    public void SoftDeleteProductInvalidProductIdTest()
    {
        var classToHandle = new SoftDeleteProduct
        {
            Id = null
        };
        
        var handler = (SoftDeleteProductHandler)ProductMockBuilder.CreateHandler<SoftDeleteProductHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ValidationException>().WithMessage("*Product Id*");
    }
}
