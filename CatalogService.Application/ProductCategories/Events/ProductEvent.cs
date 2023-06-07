using CatalogService.Application.Common;
using CatalogService.Application.ProductCategories.Responses;

namespace CatalogService.Application.ProductCategories.Events;

public class ProductCategoryEvent
{
    public ProductCategoryData Details { get; init; }
    public EventAction Action { get; init; }
}

