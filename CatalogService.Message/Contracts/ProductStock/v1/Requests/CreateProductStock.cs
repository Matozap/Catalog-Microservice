using System.Runtime.Serialization;
using CatalogService.Message.Contracts.Common.Interfaces;
using FluentValidation;

namespace CatalogService.Message.Contracts.ProductStock.v1.Requests;

[DataContract]
public class CreateProductStock : ICommand<ProductStockData>
{
    [DataMember(Order = 1)]
    public ProductStockData Details { get; init; }
}

public class CreateProductStockValidator : AbstractValidator<CreateProductStock>
{
    public CreateProductStockValidator()
    {
        RuleFor(x => x.Details).NotNull();
    }
}
