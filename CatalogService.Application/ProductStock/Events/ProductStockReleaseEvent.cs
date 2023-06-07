using CatalogService.Application.Common;
using CatalogService.Application.ProductStock.Responses;

namespace CatalogService.Application.ProductStock.Events;

public class ProductStockReleaseEvent
{
    public ReleaseProductStockResponse Details { get; init; }
    public EventAction Action { get; init; }
}

