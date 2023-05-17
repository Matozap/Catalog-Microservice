using System.Collections.Generic;
using System.Runtime.Serialization;
using CatalogService.Message.Contracts.Common.Interfaces;
using FluentValidation;

namespace CatalogService.Message.Contracts.ProductCategories.v1.Requests;

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
