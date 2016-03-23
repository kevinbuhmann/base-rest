using System;

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
    }
}
