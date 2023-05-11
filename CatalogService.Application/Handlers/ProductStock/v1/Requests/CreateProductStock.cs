using FluentValidation;
using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.ProductStock.v1;

namespace CatalogService.Application.Handlers.ProductStock.v1.Requests;

public class CreateProductStock : ICommand<ProductStockData>
{
    public ProductStockData Details { get; init; }
}

public class CreateProductStockValidator : AbstractValidator<CreateProductStock>
{
    public CreateProductStockValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Name)
            .NotNull().NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");
        RuleFor(x => x.Details.ProductImageId)
            .NotNull().NotEmpty().WithMessage("ProductImage id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
