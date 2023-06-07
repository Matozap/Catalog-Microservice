using CatalogService.Application.Common;
using CatalogService.Application.ProductImages.Responses;

namespace CatalogService.Application.ProductImages.Events;

public class ProductImageEvent
{
    public ProductImageData Details { get; init; }
    public EventAction Action { get; init; }
}

