using System.Collections.Generic;

namespace CatalogService.Domain;

public class ProductImage : EntityBase
{
    public string Code { get; set; }
    public string Name { get; set; }
    public bool Disabled { get; set; }
    public string ProductId { get; set; }
    public virtual Product Product { get; set; }
    public virtual List<ProductStock> ProductStock { get; set; }
}
