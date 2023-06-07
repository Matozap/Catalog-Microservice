using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using CatalogService.Application.Common;
using CatalogService.Application.ProductStock.Requests;
using CatalogService.Application.ProductStock.Responses;

namespace CatalogService.Application.ProductStock;

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
