using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BaseRest.Extensions
{
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

        public static object InstantiateFromStringArray(this Type type, string[] paramArray)
        {
            object result;

            var constructors = type.GetConstructors()
                .Where(i => i.GetParameters().Count() == paramArray.Length
                && i.GetParameters().All(j => j.ParameterType.IsConvertibleType()));
            if (constructors.Count() == 1)
            {
                ParameterInfo[] parameterInfo = constructors.First().GetParameters();
                List<object> parameters = new List<object>();
                for (int i = 0; i < paramArray.Length; i++)
                {
                    parameters.Add(paramArray[i].Replace(@"\_", "_").ConvertTo(parameterInfo[i].ParameterType));
                }

                result = constructors.First().Invoke(parameters.ToArray());
            }
            else if (constructors.Any())
            {
                throw new Exception("Multiple ambiguous constructors found.");
            }
            else
            {
                throw new Exception("No constructor found.");
            }

            return result;
        }

        public static bool IsConvertibleType(this Type type)
        {
            return type.IsConvertibleTo() || (type.IsArray && type.GetElementType().IsConvertibleTo());
        }

        public static bool IsConvertibleTo(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            Type innerType = Nullable.GetUnderlyingType(type);
            type = innerType ?? type;

            // These types exactly match IConvertible
            Type[] types = new Type[]
            {
                typeof(bool),
                typeof(byte),
                typeof(char),
                typeof(DateTime),
                typeof(decimal),
                typeof(double),
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(sbyte),
                typeof(float),
                typeof(string),
                typeof(object),
                typeof(ushort),
                typeof(uint),
                typeof(ulong)
            };
            return types.Contains(type) || type.IsEnum;
        }
    }
}
