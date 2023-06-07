using System.Runtime.Serialization;
using CatalogService.Application.ProductImages.Responses;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.ProductImages.Requests;

[DataContract]
public class SoftDeleteProductImage : ICommand<ProductImageData>
{
    [DataMember(Order = 1)]
    public string Id { get; init; }
}

public class SoftDeleteProductImageValidator : AbstractValidator<SoftDeleteProductImage>
{
    public SoftDeleteProductImageValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
