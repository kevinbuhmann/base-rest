using BaseRest.General;

namespace BaseRest.Boundary
{
    public interface IConverter<TDmn, TDto, TPermissions>
        where TDmn : class, IDomain
        where TDto : class, IDto, new()
        where TPermissions : IPermissions
    {
        TDto Convert(TDmn domain, TPermissions permissions, string[] includes, DeletedState deleteState);
    }
}
