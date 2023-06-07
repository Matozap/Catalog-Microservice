using CatalogService.Application.Common;
using CatalogService.Application.Products.Responses;

namespace CatalogService.Application.Products.Events;

public class ProductEvent
{
    public ProductData Details { get; init; }
    public EventAction Action { get; init; }
}

