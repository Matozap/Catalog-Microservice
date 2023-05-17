using System.Runtime.Serialization;

namespace CatalogService.Message.Contracts.ProductStock.v1.Responses;

[DataContract]
public class BookProductStockResponse
{
    [DataMember(Order = 1)]
    public string ProductId { get; init; }
    [DataMember(Order = 2)]
    public decimal Value { get; init; }
    [DataMember(Order = 3)]
    public bool Success { get; init; }
    [DataMember(Order = 4)]
    public string StatusMessage { get; init; }
}

