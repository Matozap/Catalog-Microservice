using System.Runtime.Serialization;

namespace CatalogService.Message.Contracts.ProductStock.v1;

[DataContract]
public class ProductStockData
{
    [DataMember(Order = 1)]
    public string Id { get; set; }
    [DataMember(Order = 2)]
    public decimal Current { get; set; }
    [DataMember(Order = 3)]
    public decimal Booked { get; set; }
    [DataMember(Order = 4)]
    public decimal Previous { get; set; }
    [DataMember(Order = 5)]
    public string ProductId { get; set; }
}
