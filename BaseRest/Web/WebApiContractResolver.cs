using BaseRest.Boundary;
using BaseRest.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BaseRest.Web
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
                        if (dto.ExcludedProperties != null && dto.ExcludedProperties.Length > 0)
                        {
                            string lowerPropertyName = property.PropertyName.ToLower();
                            return !dto.ExcludedProperties.Any(p => p.ToLower() == lowerPropertyName);
                        }
                        return true;
                    };
                }
            }

            return property;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            if (properties != null)
                return properties.OrderBy(p => p.DeclaringType.CountInheritanceDepth()).ToList();
            return properties;
        }
    }
}
