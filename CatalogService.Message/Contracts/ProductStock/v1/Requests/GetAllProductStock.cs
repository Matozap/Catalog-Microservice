using System.Collections.Generic;
using System.Runtime.Serialization;
using CatalogService.Message.Contracts.Common.Interfaces;
using FluentValidation;

namespace CatalogService.Message.Contracts.ProductStock.v1.Requests;

[DataContract]
public class GetAllProductStock : IQuery<List<ProductStockData>>
{
    [DataMember(Order = 1)]
    public string ProductStockId { get; init; }
}

public class GetAllProductStockValidator : AbstractValidator<GetAllProductStock>
{
    public GetAllProductStockValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.ProductStockId)
            .NotNull().NotEmpty().WithMessage("ProductImage id is required")
            .MaximumLength(36).WithMessage("ProductImage id cannot exceed 36 characters");
    }
}
