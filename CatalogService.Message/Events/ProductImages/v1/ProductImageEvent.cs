using CatalogService.Message.Contracts.ProductImages.v1;

namespace CatalogService.Message.Events.ProductImages.v1;

public class ProductImageEvent
{
    public ProductImageData Details { get; init; }
    public EventAction Action { get; init; }
}

