using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using CatalogService.Application.Common;
using CatalogService.Application.ProductCategories.Responses;

namespace CatalogService.Application.ProductCategories;

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