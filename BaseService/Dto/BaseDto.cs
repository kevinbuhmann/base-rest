using BaseService.General;

namespace BaseService.Dto
{
    public abstract class BaseDto : IDto
    {
        public int Id { get; set; }

        public string[] ExcludedProperties { get; set; }
    }
}
