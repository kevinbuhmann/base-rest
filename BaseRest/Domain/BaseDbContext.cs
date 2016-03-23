using BaseRest.Boundary;
using BaseRest.General;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace BaseRest.Domain
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
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
