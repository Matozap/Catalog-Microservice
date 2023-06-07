using System.Collections.Generic;
using System.Runtime.Serialization;
using CatalogService.Application.ProductCategories.Responses;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.ProductCategories.Requests;

[DataContract]
public class GetAllProductCategories : IQuery<List<ProductCategoryData>>
{
    
}

public class GetAllProductCategoryStockValidator : AbstractValidator<GetAllProductCategories>
{
    public GetAllProductCategoryStockValidator()
    {
        RuleFor(x => x).NotNull();
    }
}
