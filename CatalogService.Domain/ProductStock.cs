namespace CatalogService.Domain;

public class ProductStock : EntityBase
{
    public string Name { get; set; }
    public bool Disabled { get; set; }
    public string ProductImageId { get; set; }
    public virtual ProductImage ProductImage { get; set; }
}
