using System.Runtime.Serialization;
using CatalogService.Application.Products.Responses;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.Products.Requests;

[DataContract]
public class CreateProduct : ICommand<ProductData>
{
    [DataMember(Order = 1)]
    public ProductData Details { get; init; }
}

public class CreateProductValidator : AbstractValidator<CreateProduct>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Name)
            .NotNull().NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");
        RuleFor(x => x.Details.Sku)
            .NotNull().NotEmpty().WithMessage("Sku is required")
            .MaximumLength(36).WithMessage("Sku cannot exceed 36 characters");
    }
}
