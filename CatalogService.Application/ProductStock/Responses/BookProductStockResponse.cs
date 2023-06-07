using System.Runtime.Serialization;

namespace CatalogService.Application.ProductStock.Responses;

[DataContract]
public class BookProductStockResponse
{
    [DataMember(Order = 1)]
    public string Id { get; init; }
    [DataMember(Order = 2)]
    public string ProductId { get; init; }
    [DataMember(Order = 3)]
    public decimal Value { get; init; }
    [DataMember(Order = 4)]
    public bool Success { get; init; }
    [DataMember(Order = 5)]
    public string StatusMessage { get; set; }
}

