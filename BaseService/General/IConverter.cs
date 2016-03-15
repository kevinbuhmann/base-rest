using BaseDomain.General;
using System.Collections.Generic;

namespace BaseService.General
{
    public interface IConverter<TDmn, TDto, TPermissions>
        where TDmn : IDomain
        where TDto : IDto
        where TPermissions : IPermissions
    {
        TDto Convert(TDmn domain, TPermissions permissions, string[] includes);

        TDmn Create(TDto dto, TPermissions permissions);

        void Update(TDmn domain, TDto dto, TPermissions permissions);
    }
}
