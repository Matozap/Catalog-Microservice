using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Products.Commands;
using CatalogService.Application.Products.Requests;
using CatalogService.Application.Products.Responses;
using FluentAssertions;
using CatalogService.Test.MockBuilder;
using Xunit;

namespace CatalogService.Test.Commands.Products.v1;

public class CreateProductTests
{
    [Fact]
    public async Task CreateProductTest()
    {
        var classToHandle = new CreateProduct
        {
            Details = ProductMockBuilder.GenerateMockProductDtoList(1).First()
        };

        var handler = (CreateProductHandler)ProductMockBuilder.CreateHandler<CreateProductHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull().And.BeOfType<ProductData>();
    }

    [Fact]
    public void CreateProductInvalidNameTest()
    {
        var resultDto = ProductMockBuilder.GenerateMockProductDtoList(1).First();
        resultDto.Name = "";
        
        var classToHandle = new CreateProduct
        {
            Details = resultDto
        };
        
        var handler = (CreateProductHandler)ProductMockBuilder.CreateHandler<CreateProductHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ValidationException>().WithMessage("*Name*");
    }
}
