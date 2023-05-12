using CatalogService.Message.Contracts.ProductCategories.v1;

namespace CatalogService.Message.Events.ProductCategories.v1;

public class ProductCategoryEvent
{
    public ProductCategoryData Details { get; init; }
    public EventAction Action { get; init; }
}

