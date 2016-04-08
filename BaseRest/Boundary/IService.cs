using System.Collections.Generic;

namespace BaseRest.Boundary
{
    public interface IService<TDmn, TDto, TConverter, TPermissions>
        where TDmn : class, IDomain
        where TDto : class, IDto
        where TConverter : IConverter<TDmn, TDto, TPermissions>, new()
        where TPermissions : IPermissions
    {
        TDto Get(int id, string[] includes = null);

        TDto[] Get(IEnumerable<int> ids, string[] includes = null);

        TDto Create(TDto dto);

        bool Update(int id, TDto dto);

        bool Delete(TDto dto);

        void SetPermissions(TPermissions permissions);
    }
}
