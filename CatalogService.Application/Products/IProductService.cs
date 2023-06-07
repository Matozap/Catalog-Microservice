using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using CatalogService.Application.Common;
using CatalogService.Application.Products.Responses;

namespace CatalogService.Application.Products;

[ServiceContract]
public interface IProductService
{
    [OperationContract] Task<List<ProductData>> GetAll(StringWrapper id);
    [OperationContract] Task<ProductData> Get(StringWrapper id);
    [OperationContract] Task<ProductData> Create(ProductData data);
    [OperationContract] Task<ProductData> Update(ProductData data);
    [OperationContract] Task Disable(StringWrapper id);
    [OperationContract] Task Delete(StringWrapper id);
}
