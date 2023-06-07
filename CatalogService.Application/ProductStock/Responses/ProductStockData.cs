using System.Runtime.Serialization;

namespace CatalogService.Application.ProductStock.Responses;

[DataContract]
public class ProductStockData
{
    [DataMember(Order = 1)]
    public string Id { get; set; }
    [DataMember(Order = 2)]
    public decimal Current { get; set; }
    [DataMember(Order = 3)]
    public decimal Previous { get; set; }
    [DataMember(Order = 4)]
    public decimal Booked { get; set; }
    [DataMember(Order = 5)]
    public decimal Released { get; set; }
    [DataMember(Order = 6)]
    public string Action { get; set; }
    [DataMember(Order = 7)]
    public decimal ActionValue { get; set; }
    [DataMember(Order = 8)]
    public string ProductId { get; set; }
}
