using System.Runtime.Serialization;
using CatalogService.Application.ProductImages.Responses;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.ProductImages.Requests;

[DataContract]
public class CreateProductImage : ICommand<ProductImageData>
{
    [DataMember(Order = 1)]
    public ProductImageData Details { get; init; }
}

public class CreateProductImageValidator : AbstractValidator<CreateProductImage>
{
    public CreateProductImageValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Url)
            .NotNull().NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");
        RuleFor(x => x.Details.Title)
            .NotNull().NotEmpty().WithMessage("Code is required")
            .MaximumLength(36).WithMessage("Code cannot exceed 36 characters");
        RuleFor(x => x.Details.ProductId)
            .NotNull().NotEmpty().WithMessage("Product id is required")
            .MaximumLength(36).WithMessage("Product id cannot exceed 36 characters");
    }
}
