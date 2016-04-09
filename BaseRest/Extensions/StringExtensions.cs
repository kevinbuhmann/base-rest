﻿using System;
using System.Text.RegularExpressions;

namespace BaseRest.Extensions
{
    public static class StringExtensions
    {
        public static string CamelCaseToSplitLower(this string source)
        {
            return Regex.Replace(source, "([A-Z_])", "-$1", RegexOptions.Compiled).ToLower().Trim(new char[] { '-' });
        }

        public static object ConvertTo(this string value, Type type)
        {
            object result;

            if (type.IsConvertibleType() == false)
            {
                throw new Exception("type not convertible");
            }

            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                string[] values = value.Split(',');
                Array array = Array.CreateInstance(elementType, values.Length);
                for (int i = 0; i < values.Length; i++)
                {
                    array.SetValue(ConvertTo(values[i], elementType), i);
                }

                result = array;
            }
            else
            {
                Type innerType = Nullable.GetUnderlyingType(type);
                if (innerType == null)
                {
                    result = type.IsEnum ?
                        Enum.Parse(type, value, true) :
                        Convert.ChangeType(value, type);
                }
                else if (string.IsNullOrEmpty(value))
                {
                    result = null;
                }
                else
                {
                    result = Convert.ChangeType(value, innerType);
                }
            }

            return result;
        }
    }
}
