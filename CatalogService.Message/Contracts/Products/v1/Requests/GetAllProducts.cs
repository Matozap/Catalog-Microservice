using System.Collections.Generic;
using System.Runtime.Serialization;
using CatalogService.Message.Contracts.Common.Interfaces;
using FluentValidation;

namespace CatalogService.Message.Contracts.Products.v1.Requests;

[DataContract]
public class GetAllProducts : IQuery<List<ProductData>>
{
    [DataMember(Order = 1)]
    public string Id { get; init; }
}

public class GetAllProductStockValidator : AbstractValidator<GetAllProducts>
{
    public GetAllProductStockValidator()
    {
        RuleFor(x => x).NotNull();
    }
}
