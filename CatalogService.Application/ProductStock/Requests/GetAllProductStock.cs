using System.Collections.Generic;
using System.Runtime.Serialization;
using CatalogService.Application.ProductStock.Responses;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.ProductStock.Requests;

[DataContract]
public class GetAllProductStock : IQuery<List<ProductStockData>>
{
    [DataMember(Order = 1)]
    public string ProductId { get; init; }
}

public class GetAllProductStockValidator : AbstractValidator<GetAllProductStock>
{
    public GetAllProductStockValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.ProductId)
            .NotNull().NotEmpty().WithMessage("ProductImage id is required")
            .MaximumLength(36).WithMessage("ProductImage id cannot exceed 36 characters");
    }
}
