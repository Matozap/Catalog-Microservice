using System.Runtime.Serialization;

namespace CatalogService.Message.Contracts.ProductImages.v1;

[DataContract]
public class ProductImageData
{
    [DataMember(Order = 1)]
    public string Id { get; set; }
    [DataMember(Order = 2)]
    public string Title { get; init; }
    [DataMember(Order = 3)]
    public string Url { get; set; }
    [DataMember(Order = 4)]
    public string ProductId { get; set; }
}
