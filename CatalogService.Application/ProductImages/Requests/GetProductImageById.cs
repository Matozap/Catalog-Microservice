using System.Runtime.Serialization;
using CatalogService.Application.ProductImages.Responses;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.ProductImages.Requests;

[DataContract]
public class GetProductImageById : IQuery<ProductImageData>
{
    [DataMember(Order = 1)]
    public string Id { get; init; }
}

public class GetProductImageByIdValidator : AbstractValidator<GetProductImageById>
{
    public GetProductImageByIdValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
