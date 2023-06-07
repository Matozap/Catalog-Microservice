using System.Runtime.Serialization;
using CatalogService.Application.ProductStock.Responses;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.ProductStock.Requests;

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
