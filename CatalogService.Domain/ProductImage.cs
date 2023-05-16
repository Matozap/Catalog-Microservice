namespace CatalogService.Domain;

public class ProductImage : EntityBase
{
    public string Title { get; set; }
    public string Url { get; set; }
    public bool Disabled { get; set; }
    public string ProductId { get; set; }
    public virtual Product Product { get; set; }
}
