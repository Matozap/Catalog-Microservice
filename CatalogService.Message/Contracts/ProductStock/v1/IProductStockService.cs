using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using CatalogService.Message.Contracts.Common;
using CatalogService.Message.Contracts.ProductStock.v1.Requests;
using CatalogService.Message.Contracts.ProductStock.v1.Responses;

namespace CatalogService.Message.Contracts.ProductStock.v1;

[ServiceContract]
public interface IProductStockService
{
    [OperationContract] Task<List<ProductStockData>> GetAll(StringWrapper productImageId);
    [OperationContract] Task<ProductStockData> Get(StringWrapper id);
    [OperationContract] Task<ProductStockData> Create(ProductStockData data);
    [OperationContract] Task<ProductStockData> Update(UpdateProductStock data);
    [OperationContract] Task Disable(StringWrapper id);
    [OperationContract] Task Delete(StringWrapper id);
    [OperationContract] Task<BookProductStockResponse> Book(BookProductStock data);
    [OperationContract] Task<ReleaseProductStockResponse> Release(ReleaseProductStock data);
}
