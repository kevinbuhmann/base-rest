namespace BaseRest.Boundary
{
    public interface IDto
    {
        int Id { get; set; }

        bool IsDeleted { get; set; }

        string[] ExcludedProperties { get; set; }
    }
}