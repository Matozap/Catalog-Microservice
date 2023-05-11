using System.Threading.Tasks;

namespace CatalogService.Application.Events.Publishers;

public interface IOutboxPublisher
{
    Task PublishOutboxAsync();
}
