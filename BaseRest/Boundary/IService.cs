using BaseRest.Queryable;
using System.Collections.Generic;

namespace BaseRest.Boundary
{
    public interface IService<TDmn, TDto, TConverter, TPermissions>
        where TDmn : class, IDomain
        where TDto : class, IDto
        where TConverter : IConverter<TDmn, TDto, TPermissions>, new()
        where TPermissions : IPermissions
    {
        Queryable<TDmn, TDto, TConverter, TPermissions> Get(int id);

        Queryable<TDmn, TDto, TConverter, TPermissions> Get(IEnumerable<int> ids);

        TDto Create(TDto dto);

        bool Update(int id, TDto dto);

        bool Delete(TDto dto);

        void SetPermissions(TPermissions permissions);
    }
}
