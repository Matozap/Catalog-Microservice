using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoFixture;
using DistributedCache.Core;
using CatalogService.Application.Handlers.ProductImages.v1.Commands;
using CatalogService.Application.Handlers.ProductImages.v1.Queries;
using CatalogService.Application.Handlers.ProductImages.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using CatalogService.Message.Contracts.ProductImages.v1;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace CatalogService.Test.MockBuilder;

public static class ProductImageMockBuilder
{
    private static readonly Fixture Fixture = new();

    private static IRepository GenerateMockRepository(ProductImage catalog = null, int rowCount = 100)
    {
        var mockProductImage = catalog ?? GenerateMockProductImage();
        var mockProductImages = GenerateMockDomainProductImageList(rowCount);
        
        var repository = Substitute.For<IRepository>();
        
        repository.AddAsync(Arg.Any<ProductImage>()).Returns(mockProductImage);
        repository.UpdateAsync(mockProductImage).Returns(mockProductImage);
        repository.DeleteAsync(mockProductImage).Returns(mockProductImage);
        
        repository.GetAsSingleAsync(Arg.Any<Expression<Func<ProductImage, bool>>>(), Arg.Any<Expression<Func<ProductImage, string>>>(), 
            Arg.Any<Expression<Func<ProductImage, string>>>(), Arg.Any<Expression<Func<ProductImage, ProductImage>>>(), Arg.Any<bool>()).Returns(mockProductImage);
        repository.GetAsListAsync(Arg.Any<Expression<Func<ProductImage, bool>>>(), Arg.Any<Expression<Func<ProductImage, string>>>(), 
            Arg.Any<Expression<Func<ProductImage, string>>>(), Arg.Any<Expression<Func<ProductImage, ProductImage>>>(), Arg.Any<bool>()).Returns(mockProductImages);
        return repository;
    }

    private static ICache GenerateMockObjectCache()
    {
        var cache = Substitute.For<ICache>();
        return cache;
    }

    private static List<ProductImage> GenerateMockDomainProductImageList(int count)
    {
        return Fixture.Build<ProductImage>()
            .Without(s => s.ProductStock)
            .With(s => s.Product, () => new Product())
            .With(s => s.LastUpdateDate, () => DateTime.Now)
            .With(s => s.LastUpdateUserId, () => "Test")
            .CreateMany(count)
            .ToList();
    }

    public static List<ProductImageData> GenerateMockProductImageDtoList(int count)
    {
        return Fixture.Build<ProductImageData>()
            .Without(s => s.ProductStock)
            .CreateMany(count).ToList();
    }

    public static ProductImage GenerateMockProductImage()
    {
        return Fixture.Build<ProductImage>()
            .Without(s => s.ProductStock)
            .Without(s => s.Product)
            .With(s => s.LastUpdateDate, () => DateTime.Now)
            .With(s => s.LastUpdateUserId, () => "Test")
            .Create();
    }


    public static object CreateHandler<T>()
    {
        var response = GenerateMockProductImageDtoList(1).FirstOrDefault();
        var catalog = GenerateMockProductImage();
        
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetProductImageById>()).Returns(response);

        if (typeof(T) == typeof(UpdateProductImageHandler))
        {
            return new UpdateProductImageHandler(NullLogger<UpdateProductImageHandler>.Instance,
                GenerateMockRepository(catalog));
        }
        
        if (typeof(T) == typeof(SoftDeleteProductImageHandler))
        {
            return new SoftDeleteProductImageHandler(NullLogger<SoftDeleteProductImageHandler>.Instance,
                GenerateMockRepository(catalog));
        }
        
        if (typeof(T) == typeof(DeleteProductImageHandler))
        {
            return new DeleteProductImageHandler(NullLogger<DeleteProductImageHandler>.Instance,
                GenerateMockRepository(catalog));
        }
        
        if (typeof(T) == typeof(CreateProductImageHandler))
        {
            var repository = GenerateMockRepository(catalog);
            repository.GetAsSingleAsync(Arg.Any<Expression<Func<ProductImage, bool>>>(), Arg.Any<Expression<Func<ProductImage, string>>>(), 
                Arg.Any<Expression<Func<ProductImage, string>>>(), Arg.Any<Expression<Func<ProductImage, ProductImage>>>(), Arg.Any<bool>()).Returns((ProductImage)null);
            return new CreateProductImageHandler(NullLogger<CreateProductImageHandler>.Instance,
                repository);
        }
        
        if (typeof(T) == typeof(GetAllProductImagesHandler))
        {
            return new GetAllProductImagesHandler(GenerateMockObjectCache(),
                NullLogger<GetAllProductImagesHandler>.Instance,
                GenerateMockRepository(rowCount: 10));
        }
        
        if (typeof(T) == typeof(GetProductImageByIdHandler))
        {
            return new GetProductImageByIdHandler(GenerateMockRepository(catalog),
                GenerateMockObjectCache(),
                NullLogger<GetProductImageByIdHandler>.Instance);
        }
        
        return null;
    } 
}
