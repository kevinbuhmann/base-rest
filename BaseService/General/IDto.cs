using System.Collections.Generic;

namespace BaseService.General
{
    public interface IDto
    {
        int Id { get; set; }

        List<string> ExcludedProperties { get; }
    }
}