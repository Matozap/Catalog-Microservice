namespace CatalogService.Domain;

public class ProductStock : EntityBase
{
    public decimal Current { get; set; }
    public decimal Booked { get; set; }
    public decimal Previous { get; set; }
    public string ProductId { get; set; }
    public virtual Product Product { get; set; }
}
