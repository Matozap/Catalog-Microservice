using System.Runtime.Serialization;
using FluentValidation;
using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.ProductStock.v1;

namespace CatalogService.Application.Handlers.ProductStock.v1.Requests;

[DataContract]
public class UpdateProductStock : ICommand<ProductStockData>
{
    [DataMember(Order = 1)]
    public string Id { get; init; }
    [DataMember(Order = 2)]
    public decimal Value { get; init; }
}

public class UpdateProductStockValidator : AbstractValidator<UpdateProductStock>
{
    public UpdateProductStockValidator()
    {
        RuleFor(x => x.Value).NotEqual(0).WithMessage("Stock value must be different than 0");
    }
}
