namespace GiveLoveFirst.Boundary
{
    public interface IConverter<TDmn, TDto>
        where TDmn : IDomain
        where TDto : IDto
    {
        TDto Convert(TDmn domain);

        TDmn Create(TDto dto);

        void Update(TDmn domain, TDto dto);
    }
}
