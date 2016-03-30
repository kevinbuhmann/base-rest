using BaseRest.Boundary;
using BaseRest.Domain;
using System.Data.Entity;
using System.Linq;

namespace BaseRest.Service.Services
{
    public abstract class BaseService<TDmn, TDto, TConverter, TContext, TPermissions> : IService<TDmn, TDto, TConverter, TPermissions>
        where TDmn : class, IDomain
        where TDto : class, IDto
        where TConverter : IConverter<TDmn, TDto, TPermissions>, new()
        where TContext : BaseDbContext, new()
        where TPermissions : IPermissions
    {
        private readonly TContext dbContext;

        protected DbSet<TDmn> DbSet { get; }

        protected TConverter Converter { get; }

        protected TPermissions Permissions { get; private set; }

        public BaseService(TPermissions permissions)
        {
            this.Permissions = permissions;

            this.dbContext = new TContext();
            this.DbSet = dbContext.Set<TDmn>();
            this.Converter = new TConverter();
        }

        public virtual TDto Get(int id, string[] includes = null)
        {
            IQueryable<TDmn> query = this.DbSet
                .Where(dmn => !dmn.UtcDateDeleted.HasValue && dmn.Id == id);

            foreach (string include in includes ?? new string[0])
            {
                query.Include(include);
            }

            TDmn domain = query.FirstOrDefault();

            return domain != null ?
                this.Converter.Convert(domain, this.Permissions, includes) : null;
        }

        public virtual TDto[] GetAll(string[] includes = null)
        {
            IQueryable<TDmn> query = this.DbSet
                .Where(dmn => !dmn.UtcDateDeleted.HasValue);

            foreach (string include in includes ?? new string[0])
            {
                query.Include(include);
            }

            TDmn[] domains = query.ToArray();

            return domains
                .Select(dmn => this.Converter.Convert(dmn, this.Permissions, includes))
                .ToArray();
        }

        public TDto Create(TDto dto)
        {
            TDmn domain = this.ConstructDomain(dto);
            if (domain != null)
            {
                this.dbContext.Entry(domain).State = EntityState.Added;
                this.dbContext.SaveChanges();

                return this.OnCreate(this.Get(domain.Id));
            }

            return null;
        }

        public virtual TDto OnCreate(TDto dto)
        {
            return dto;
        }

        public bool Update(int id, TDto dto)
        {
            TDmn domain = this.DbSet.Find(id);
            if (domain != null)
            {
                dto.Id = id;
                this.UpdateDomain(domain, dto);
                this.dbContext.SaveChanges();
            }

            return domain != null;
        }

        public virtual bool Delete(TDto dto)
        {
            TDmn domain = this.DbSet.Find(dto.Id);
            if (domain != null)
            {
                this.DbSet.Remove(domain);
                this.dbContext.SaveChanges();

                return true;
            }

            return false;
        }

        public virtual void SetPermissions(TPermissions permissions)
        {
            this.Permissions = permissions;
        }

        protected abstract TDmn ConstructDomain(TDto dto);

        protected abstract void UpdateDomain(TDmn domain, TDto dto);
    }
}
