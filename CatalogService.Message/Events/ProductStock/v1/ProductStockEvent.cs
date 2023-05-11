using CatalogService.Message.Contracts.ProductStock.v1;

namespace CatalogService.Message.Events.ProductStock.v1;

public class ProductStockEvent
{
    public ProductStockData Details { get; init; }
    public EventAction Action { get; init; }
}

