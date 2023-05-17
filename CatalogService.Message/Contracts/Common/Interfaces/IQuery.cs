using MediatR;

namespace CatalogService.Message.Contracts.Common.Interfaces;

public interface IQuery<out TIQueryResult> : IRequest<TIQueryResult>
{
    
}
