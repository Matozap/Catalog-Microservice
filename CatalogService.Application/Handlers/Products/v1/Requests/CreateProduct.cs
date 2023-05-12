using FluentValidation;
using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.Products.v1;

namespace CatalogService.Application.Handlers.Products.v1.Requests;

public class CreateProduct : ICommand<ProductData>
{
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
