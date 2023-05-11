using System.Collections.Generic;
using FluentValidation;
using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.Products.v1;

namespace CatalogService.Application.Handlers.Products.v1.Requests;

public class GetAllProducts : IQuery<List<ProductData>>
{
    
}

public class GetAllProductStockValidator : AbstractValidator<GetAllProducts>
{
    public GetAllProductStockValidator()
    {
        RuleFor(x => x).NotNull();
    }
}
