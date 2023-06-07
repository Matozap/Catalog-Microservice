using System.Collections.Generic;
using System.Runtime.Serialization;
using CatalogService.Application.Products.Responses;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.Products.Requests;

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
