using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using CatalogService.Message.Contracts.Common;

namespace CatalogService.Message.Contracts.ProductImages.v1;

[ServiceContract]
public interface IProductImageService
{
    [OperationContract] Task<List<ProductImageData>> GetAll(StringWrapper productId);
    [OperationContract] Task<ProductImageData> Get(StringWrapper id);
    [OperationContract] Task<ProductImageData> Create(ProductImageData data);
    [OperationContract] Task<ProductImageData> Update(ProductImageData data);
    [OperationContract] Task Disable(StringWrapper id);
    [OperationContract] Task Delete(StringWrapper id);
}
