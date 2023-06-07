using System.Runtime.Serialization;
using CatalogService.Application.ProductImages.Responses;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.ProductImages.Requests;

[DataContract]
public class UpdateProductImage : ICommand<ProductImageData>
{
    [DataMember(Order = 1)]
    public ProductImageData Details { get; init; }
}

public class UpdateProductImageValidator : AbstractValidator<UpdateProductImage>
{
    public UpdateProductImageValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
