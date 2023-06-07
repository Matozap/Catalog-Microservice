using CatalogService.Application.Common;
using CatalogService.Application.ProductStock.Responses;

namespace CatalogService.Application.ProductStock.Events;

public class ProductStockBookEvent
{
    public BookProductStockResponse Details { get; init; }
    public EventAction Action { get; init; }
}

