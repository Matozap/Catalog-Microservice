using System.Collections.Generic;
using System.Runtime.Serialization;
using CatalogService.Message.Contracts.Products.v1;

namespace CatalogService.Message.Contracts.ProductCategories.v1;

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