using MediatR;

namespace CatalogService.Application.Interfaces;

public interface IQuery<out TIQueryResult> : IRequest<TIQueryResult>
{
    
}
