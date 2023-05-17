using MediatR;

namespace CatalogService.Message.Contracts.Common.Interfaces;

public interface ICommand<out TCommandResult> : IRequest<TCommandResult>
{
}
