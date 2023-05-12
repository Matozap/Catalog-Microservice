using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using CatalogService.Message.Contracts.Common;

namespace CatalogService.Message.Contracts.ProductCategories.v1;

[ServiceContract]
public interface IProductCategoryService
{
    [OperationContract] Task<List<ProductCategoryData>> GetAll(StringWrapper id);
    [OperationContract] Task<ProductCategoryData> Get(StringWrapper id);
    [OperationContract] Task<ProductCategoryData> Create(ProductCategoryData data);
    [OperationContract] Task<ProductCategoryData> Update(ProductCategoryData data);
    [OperationContract] Task Disable(StringWrapper id);
    [OperationContract] Task Delete(StringWrapper id);
}