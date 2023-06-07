using CatalogService.Application.Common;
using CatalogService.Application.ProductStock.Responses;

namespace CatalogService.Application.ProductStock.Events;

public class ProductStockEvent
{
    public ProductStockData Details { get; init; }
    public EventAction Action { get; init; }
}

