using System.Collections.Generic;
using System.Runtime.Serialization;
using CatalogService.Message.Contracts.ProductStock.v1;

namespace CatalogService.Message.Contracts.ProductImages.v1;

[DataContract]
public class ProductImageData
{
    [DataMember(Order = 1)]
    public string Id { get; set; }
    [DataMember(Order = 2)]
    public string Code { get; init; }
    [DataMember(Order = 3)]
    public string Name { get; set; }
    [DataMember(Order = 4)]
    public string ProductId { get; set; }
    [DataMember(Order = 5)]
    public IReadOnlyCollection<ProductStockData> ProductStock { get; set; }
}
