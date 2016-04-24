using BaseRest.Boundary;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BaseRest.Domain
{
    [DbConfigurationType(typeof(BaseDbConfiguration))]
    public class BaseDbContext : DbContext
    {
        public BaseDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Database.Log = (logText) => Debug.WriteLine(logText);
        }

        public override int SaveChanges()
        {
            this.HandleEntryStates();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            this.HandleEntryStates();

            return base.SaveChangesAsync();
        }

        private void HandleEntryStates()
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
        }
    }
}
