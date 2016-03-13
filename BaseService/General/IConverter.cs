using BaseDomain.General;

namespace BaseService.General
{
    public interface IConverter<TDmn, TDto, TPermissions>
        where TDmn : IDomain
        where TDto : IDto
        where TPermissions : IPermissions
    {
        TDto Convert(TDmn domain);

        TDmn Create(TDto dto);

        void Update(TDmn domain, TDto dto);

        void SetPermissions(TPermissions permissions);
    }
}
