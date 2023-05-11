using System.Collections.Generic;
using FluentValidation;
using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.ProductStock.v1;

namespace CatalogService.Application.Handlers.ProductStock.v1.Requests;

public class GetAllProductStock : IQuery<List<ProductStockData>>
{
    public string ProductImageId { get; init; }
}

public class GetAllProductStockValidator : AbstractValidator<GetAllProductStock>
{
    public GetAllProductStockValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.ProductImageId)
            .NotNull().NotEmpty().WithMessage("ProductImage id is required")
            .MaximumLength(36).WithMessage("ProductImage id cannot exceed 36 characters");
    }
}
