using BaseRest.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

            ConstructorInfo[] callableConstructors = type.GetConstructors()
                .Where(i => i.GetParameters().All(j => j.ParameterType.IsConvertibleType()))
                .ToArray();
            if (!callableConstructors.Any())
            {
                throw new Exception("Type cannot be instantiated from string array because no callable constructors exist.");
            }

            ConstructorInfo[] matchingConstructors = callableConstructors
                .Where(i => i.GetParameters().Count() == paramArray.Length)
                .ToArray();
            if (matchingConstructors.Length == 1)
            {
                ConstructorInfo constructor = matchingConstructors[0];
                ParameterInfo[] parameterInfo = constructor.GetParameters();
                List<object> parameters = new List<object>();
                for (int i = 0; i < paramArray.Length; i++)
                {
                    parameters.Add(paramArray[i].Replace(@"\_", "_").ConvertTo(parameterInfo[i].ParameterType));
                }

                result = constructor.Invoke(parameters.ToArray());
            }
            else if (matchingConstructors.Any())
            {
                throw new Exception("Type cannot be instantiated from string array because multiple matching callable constructors exist.");
            }
            else
            {
                throw new RestfulException(HttpStatusCode.BadRequest); // callable constructor(s) exist, but number of parameters is incorrect
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
