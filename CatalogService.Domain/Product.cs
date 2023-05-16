using System.Collections.Generic;

namespace CatalogService.Domain;

public class Product : EntityBase
{
    public string Sku { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ProductCategoryId { get; set; }
    public string Brand { get; set; }
    public string Dimensions { get; set; }
    public decimal Weight { get; set; }
    public bool Disabled { get; set; }
    public virtual ProductCategory ProductCategory { get; set; }
    public virtual List<ProductImage> ProductImages { get; set; }
    public virtual List<ProductStock> ProductStocks { get; set; }
}
