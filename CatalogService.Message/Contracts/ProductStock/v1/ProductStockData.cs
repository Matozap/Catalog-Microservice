using ProtoBuf;

namespace CatalogService.Message.Contracts.ProductStock.v1;

[ProtoContract]
public class ProductStockData
{
    [ProtoMember(1, DataFormat = DataFormat.WellKnown)]
    public string Id { get; set; }
    [ProtoMember(2, DataFormat = DataFormat.WellKnown)]
    public string Name { get; set; }
    [ProtoMember(3, DataFormat = DataFormat.WellKnown)]
    public string ProductImageId { get; set; }
}
