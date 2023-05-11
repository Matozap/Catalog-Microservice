using System.Collections.Generic;

namespace CatalogService.Domain;

public class Product : EntityBase
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Currency { get; set; }
    public string CurrencyName { get; set; }
    public string Region { get; set; }
    public string SubRegion { get; set; }
    public bool Disabled { get; set; }
    public virtual List<ProductImage> ProductImages { get; set; }
}
