using System.Collections.Generic;
using System.Runtime.Serialization;
using CatalogService.Application.Products.Responses;

namespace CatalogService.Application.ProductCategories.Responses;

[DataContract]
public class ProductCategoryData
{
    [DataMember(Order = 1)]
    public string Id { get; set; }
    [DataMember(Order = 2)]
    public string Name { get; set; }
    [DataMember(Order = 3)]
    public string Description { get; set; }
    [DataMember(Order = 4)]
    public bool Disabled { get; set; }
    [DataMember(Order = 5)]
    public virtual List<ProductData> Products { get; set; }
}