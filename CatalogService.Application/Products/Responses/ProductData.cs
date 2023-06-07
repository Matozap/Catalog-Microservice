using System.Collections.Generic;
using System.Runtime.Serialization;
using CatalogService.Application.ProductImages.Responses;

namespace CatalogService.Application.Products.Responses;

[DataContract]
public class ProductData
{
    [DataMember(Order = 1)]
    public string Id { get; set; }
    [DataMember(Order = 2)]
    public string Sku { get; set; }
    [DataMember(Order = 3)]
    public string Name { get; set; }
    [DataMember(Order = 4)]
    public string Description { get; set; }
    [DataMember(Order = 5)]
    public decimal Price { get; set; }
    [DataMember(Order = 6)]
    public string ProductCategoryId { get; set; }
    [DataMember(Order = 7)]
    public string Brand { get; set; }
    [DataMember(Order = 8)]
    public string Dimensions { get; set; }
    [DataMember(Order = 9)]
    public decimal Weight { get; set; }
    [DataMember(Order = 10)]
    public IReadOnlyCollection<ProductImageData> ProductImages { get; set; }
}
