using BaseDomain;
using BaseDomain.General;
using BaseService.General;
using System.Data.Entity;
using System.Linq;

namespace BaseService.Services
{
    public abstract class BaseService<TDmn, TDto, TConverter, TContext, TPermissions> : IService<TDmn, TDto, TConverter, TPermissions>
        where TDmn : class, IDomain
        where TDto : class, IDto
        where TConverter : IConverter<TDmn, TDto, TPermissions>, new()
        where TContext : BaseDbContext, new()
        where TPermissions : IPermissions
    {
        private readonly TContext dbContext;
        private readonly DbSet<TDmn> dbSet;
        private readonly TConverter converter;
        private TPermissions permissions;

        public BaseService(TPermissions permissions)
        {
            this.permissions = permissions;

            this.dbContext = new TContext();
            this.dbSet = dbContext.Set<TDmn>();
            this.converter = new TConverter();
        }

        public virtual TDto[] GetAll(string[] includes = null)
        {
            IQueryable<TDmn> query = this.dbSet
                .Where(dmn => !dmn.UtcDateDeleted.HasValue);

            foreach (string include in includes)
            {
                query.Include(include);
            }

            TDmn[] domains = query.ToArray();

            return domains
                .Select(dmn => this.converter.Convert(dmn, this.permissions, includes))
                .ToArray();
        }

        public virtual TDto Get(int id, string[] includes = null)
        {
            IQueryable<TDmn> query = this.dbSet
                .Where(dmn => !dmn.UtcDateDeleted.HasValue && dmn.Id == id);

            foreach (string include in includes)
            {
                query.Include(include);
            }

            TDmn domain = query.FirstOrDefault();

            return domain != null ?
                this.converter.Convert(domain, this.permissions, includes) : null;
        }

        public virtual TDto Create(TDto dto)
        {
            TDmn domain = this.converter.Create(dto, this.permissions);
            if (domain != null)
            {
                this.dbContext.Entry(domain).State = EntityState.Added;
                this.dbContext.SaveChanges();

                return this.Get(domain.Id);
            }

            return null;
        }

        public virtual bool Update(int id, TDto dto)
        {
            TDmn domain = this.dbSet.Find(id);
            if (domain != null)
            {
                dto.Id = id;
                this.converter.Update(domain, dto, this.permissions);
                this.dbContext.SaveChanges();
            }

            return domain != null;
        }

        public virtual bool Delete(int id)
        {
            TDmn domain = this.dbSet.Find(id);
            if (domain != null)
            {
                this.dbSet.Remove(domain);
                this.dbContext.SaveChanges();

                return true;
            }

            return false;
        }

        public void SetPermissions(TPermissions permissions)
        {
            this.permissions = permissions;
        }
    }
}
