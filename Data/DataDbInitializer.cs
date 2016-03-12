using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveLoveFirst.Data
{
    public class DataDbInitializer<TContext> : DropCreateDatabaseIfModelChanges<TContext>
        where TContext : DataDbContext
    {
    }
}
