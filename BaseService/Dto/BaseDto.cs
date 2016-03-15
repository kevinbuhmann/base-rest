using System;
using System.Collections.Generic;
using BaseService.General;

namespace BaseService.Dto
{
    public abstract class BaseDto : IDto
    {
        public int Id { get; set; }

        public List<string> ExcludedProperties { get; private set; }

        public BaseDto()
        {
            this.ExcludedProperties = new List<string>();
        }
    }
}
