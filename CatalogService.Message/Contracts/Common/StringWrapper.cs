using System.Diagnostics.CodeAnalysis;
using ProtoBuf;

namespace CatalogService.Message.Contracts.Common;

[ExcludeFromCodeCoverage]
[ProtoContract]
public class StringWrapper
{
    [ProtoMember(1, DataFormat = DataFormat.WellKnown)]
    public string Value { get; set; }
}
