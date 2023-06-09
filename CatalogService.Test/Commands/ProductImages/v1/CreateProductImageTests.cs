using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.ProductImages.Commands;
using CatalogService.Application.ProductImages.Requests;
using CatalogService.Application.ProductImages.Responses;
using FluentAssertions;
using CatalogService.Test.MockBuilder;
using Xunit;

namespace CatalogService.Test.Commands.ProductImages.v1;

public class CreateProductImageTests
{
    [Fact]
    public async Task CreateProductImageTest()
    {
        var classToHandle = new CreateProductImage
        {
            Details = ProductImageMockBuilder.GenerateMockProductImageDtoList(1).First()
        };

        var handler = (CreateProductImageHandler)ProductImageMockBuilder.CreateHandler<CreateProductImageHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull().And.BeOfType<ProductImageData>();
    }

    [Fact]
    public void CreateProductImageInvalidNameTest()
    {
        var resultDto = ProductImageMockBuilder.GenerateMockProductImageDtoList(1).First();
        resultDto.Url = "";
        
        var classToHandle = new CreateProductImage
        {
            Details = resultDto
        };
        
        var handler = (CreateProductImageHandler)ProductImageMockBuilder.CreateHandler<CreateProductImageHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());
    
        action.Should().ThrowAsync<ValidationException>().WithMessage("*Name*");
    }
}
