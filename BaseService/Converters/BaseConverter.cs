using BaseDomain.General;
using BaseService.General;
using General;
using System.Net;

namespace BaseService.Converters
{
    public abstract class BaseConverter<TDmn, TDto, TPermissions> : IConverter<TDmn, TDto, TPermissions>
        where TDmn : IDomain
        where TDto : IDto
        where TPermissions : IPermissions
    {
        protected TPermissions Permissions { get; private set; }

        public abstract TDto Convert(TDmn domain);

        public abstract TDmn Create(TDto dto);

        public virtual void Update(TDmn domain, TDto dto)
        {
            if (domain.Id == 0)
            {
                throw new RestfulException(HttpStatusCode.Conflict, "Cannot update new domain.");
            }

            if (domain.Id != dto.Id)
            {
                throw new RestfulException(HttpStatusCode.Conflict, "Domain and Dto must have the same id.");
            }
        }

        public void SetPermissions(TPermissions permissions)
        {
            this.Permissions = permissions;
        }
    }
}
