using System.Collections.Generic;
using System.Runtime.Serialization;
using CatalogService.Application.ProductImages.Responses;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.ProductImages.Requests;

[DataContract]
public class GetAllProductImages : IQuery<List<ProductImageData>>
{
    [DataMember(Order = 1)]
    public string ProductId { get; init; }
}

public class GetAllProductImagesValidator : AbstractValidator<GetAllProductImages>
{
    public GetAllProductImagesValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.ProductId)
            .NotNull().NotEmpty().WithMessage("Product id is required")
            .MaximumLength(36).WithMessage("Product id  cannot exceed 36 characters");
    }
}
