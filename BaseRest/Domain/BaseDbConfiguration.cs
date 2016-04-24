using System.Data.Entity;

namespace BaseRest.Domain
{
    public class BaseDbConfiguration : DbConfiguration
    {
        public BaseDbConfiguration()
        {
            this.SetDatabaseLogFormatter((context, writeAction) => new OneLineFormatter(context, writeAction));
        }
    }
}
