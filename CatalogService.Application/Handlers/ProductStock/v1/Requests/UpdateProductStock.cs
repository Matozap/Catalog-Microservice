using FluentValidation;
using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.ProductStock.v1;

namespace CatalogService.Application.Handlers.ProductStock.v1.Requests;

public class UpdateProductStock : ICommand<ProductStockData>
{
    public ProductStockData Details { get; init; }
}

public class UpdateProductStockValidator : AbstractValidator<UpdateProductStock>
{
    public UpdateProductStockValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
