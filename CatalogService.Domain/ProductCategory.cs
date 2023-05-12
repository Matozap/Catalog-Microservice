using System.Collections.Generic;

namespace CatalogService.Domain;

public class ProductCategory : EntityBase
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Disabled { get; set; }
    public virtual List<Product> Products { get; set; }
}