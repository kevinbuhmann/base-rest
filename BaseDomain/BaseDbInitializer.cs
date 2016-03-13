using System.Data.Entity;

namespace BaseDomain
{
    public class BaseDbInitializer<TContext> : DropCreateDatabaseIfModelChanges<TContext>
        where TContext : BaseDbContext
    {
    }
}
