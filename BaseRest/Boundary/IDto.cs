namespace BaseRest.Boundary
{
    public interface IDto
    {
        int Id { get; set; }

        string[] ExcludedProperties { get; }
    }
}