using BaseService.General;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

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

    public static class TypeExtensions
    {
        public static int CountInheritanceDepth(this Type type)
        {
            int count = 0;

            Type baseType = type;
            while (baseType != null)
            {
                count++;
                baseType = baseType.BaseType;
            }

            return count;
        }
    }
}
