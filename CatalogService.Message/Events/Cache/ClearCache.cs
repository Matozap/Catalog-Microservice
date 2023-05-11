using MediatR;

namespace CatalogService.Message.Events.Cache;

public class ClearCache : IRequest<bool>
{
    public string ProductId { get; init; }
    public string ProductImageId { get; init; }
    public string ProductImageCode { get; init; }
    public string ProductStockId { get; init; }
    public bool ClearAll { get; set; }
}
