using System.Collections.Generic;
using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.ProductCategories.v1;
using FluentValidation;

namespace CatalogService.Application.Handlers.ProductCategories.v1.Requests;

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
