using System.Data.Entity;

namespace BaseRest.Domain
{
    public class BaseDbInitializer<TContext> : DropCreateDatabaseIfModelChanges<TContext>
        where TContext : BaseDbContext
    {
    }
}
