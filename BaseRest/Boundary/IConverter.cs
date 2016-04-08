namespace BaseRest.Boundary
{
    public interface IConverter<TDmn, TDto, TPermissions>
        where TDmn : class, IDomain
        where TDto : class, IDto
        where TPermissions : IPermissions
    {
        TDto Convert(TDmn domain, TPermissions permissions, string[] includes);
    }
}
