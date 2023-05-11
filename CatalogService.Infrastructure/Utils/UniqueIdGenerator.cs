using UUIDNext;

namespace CatalogService.Infrastructure.Utils;

public static class UniqueIdGenerator
{
    public static string GenerateSequentialId()
    {
        return Uuid.NewDatabaseFriendly(UUIDNext.Database.SqlServer).ToString();
    }
}
