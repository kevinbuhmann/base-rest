using BaseRest.Boundary;

namespace BaseRest.Service.Dto
{
    public abstract class BaseDto : IDto
    {
        public int Id { get; set; }

        public bool IsDeleted { get; set; }

        public string[] ExcludedProperties { get; set; }
    }
}
