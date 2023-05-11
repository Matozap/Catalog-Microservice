using System.Collections.Generic;
using System.Runtime.Serialization;
using CatalogService.Message.Contracts.ProductImages.v1;

namespace CatalogService.Message.Contracts.Products.v1;

[DataContract]
public class ProductData
{
    [DataMember(Order = 1)]
    public string Id { get; set; }
    [DataMember(Order = 2)]
    public string Code { get; set; }
    [DataMember(Order = 3)]
    public string Name { get; set; }
    [DataMember(Order = 4)]
    public string Currency { get; set; }
    [DataMember(Order = 5)]
    public string CurrencyName { get; set; }
    [DataMember(Order = 6)]
    public string Region { get; set; }
    [DataMember(Order = 7)]
    public string SubRegion { get; set; }
    [DataMember(Order = 8)]
    public IReadOnlyCollection<ProductImageData> ProductImages { get; set; }
}
