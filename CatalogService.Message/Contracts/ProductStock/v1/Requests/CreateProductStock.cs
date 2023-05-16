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
    }
}
