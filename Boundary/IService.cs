namespace GiveLoveFirst.Boundary
{
    public interface IService<TDmn, TDto, TConverter>
        where TDmn : IDomain
        where TDto : IDto
        where TConverter : IConverter<TDmn, TDto>
    {
        TDto[] GetAll();

        TDto Get(int id);

        TDto Create(TDto dto);

        bool Update(int id, TDto dto);

        bool Delete(int id);
    }
}
