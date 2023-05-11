using CatalogService.Message.Contracts.Products.v1;

namespace CatalogService.Message.Events.Products.v1;

public class ProductEvent
{
    public ProductData Details { get; init; }
    public EventAction Action { get; init; }
}

