using GiveLoveFirst.Boundary;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace GiveLoveFirst.Data
{
    public class DataDbContext : DbContext
    {
        public DataDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public override int SaveChanges()
        {
            foreach (DbEntityEntry<IDomain> entry in this.ChangeTracker.Entries<IDomain>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Create();
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.Modify();
                }
                else if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.Delete();
                }
            }

            return base.SaveChanges();
        }
    }
}