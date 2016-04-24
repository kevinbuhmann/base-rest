namespace BaseService.General
{
    public interface IDto
    {
        int Id { get; set; }

        string[] ExcludedProperties { get; }
    }
}