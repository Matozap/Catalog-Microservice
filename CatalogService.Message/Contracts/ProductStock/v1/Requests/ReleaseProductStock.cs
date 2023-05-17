using System.Runtime.Serialization;
using CatalogService.Message.Contracts.Common.Interfaces;
using CatalogService.Message.Contracts.ProductStock.v1.Responses;
using FluentValidation;

namespace CatalogService.Message.Contracts.ProductStock.v1.Requests;

[DataContract]
public class ReleaseProductStock : ICommand<ReleaseProductStockResponse>
{
    [DataMember(Order = 1)]
    public string ProductId { get; init; }
    [DataMember(Order = 2)]
    public decimal Value { get; init; }
}

public class ReleaseProductStockValidator : AbstractValidator<ReleaseProductStock>
{
    public ReleaseProductStockValidator()
    {
        RuleFor(x => x.ProductId).NotNull();
        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage("Released value must be a positive number and greater than 0");
    }
}
