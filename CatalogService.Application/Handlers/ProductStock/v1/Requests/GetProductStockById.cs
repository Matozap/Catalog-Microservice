using FluentValidation;
using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.ProductStock.v1;

namespace CatalogService.Application.Handlers.ProductStock.v1.Requests;

public class GetProductStockById : IQuery<ProductStockData>
{
    public string Id { get; init; }
}

public class GetProductStockByIdValidator : AbstractValidator<GetProductStockById>
{
    public GetProductStockByIdValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
