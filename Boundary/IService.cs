namespace GiveLoveFirst.Boundary
{
    public interface IService<TDmn, TDto, TConverter, TPermissions>
        where TDmn : IDomain
        where TDto : IDto
        where TConverter : IConverter<TDmn, TDto, TPermissions>, new()
        where TPermissions : IPermissions
    {
        TDto[] GetAll();

        TDto Get(int id);

        TDto Create(TDto dto);

        bool Update(int id, TDto dto);

        bool Delete(int id);

        void SetPermissions(TPermissions permissions);
    }
}
