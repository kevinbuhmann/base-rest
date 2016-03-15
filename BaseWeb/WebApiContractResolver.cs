using BaseService.General;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Reflection;

namespace BaseWeb
{
    public class WebApiContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (typeof(IDto).IsAssignableFrom(property.DeclaringType))
            {
                if (property.PropertyName.ToLower() == nameof(IDto.ExcludedProperties).ToLower())
                {
                    property.ShouldSerialize = instance => false;
                }
                else
                {
                    property.ShouldSerialize = instance =>
                    {
                        IDto dto = instance as IDto;
                        string lowerPropertyName = property.PropertyName.ToLower();
                        return !dto.ExcludedProperties.Any(p => p.ToLower() == lowerPropertyName);
                    };
                }
            }

            return property;
        }
    }
}
