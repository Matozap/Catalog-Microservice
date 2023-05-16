using MediatR;

namespace CatalogService.Application.Interfaces;

public interface ICommand<out TCommandResult> : IRequest<TCommandResult>
{
}
