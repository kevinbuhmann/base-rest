using System;

namespace BaseRest.Extensions
{
    public static class ArrayExtensions
    {
        public static T RandomItem<T>(this T[] array)
        {
            Random random = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
            return array[random.Next() % array.Length];
        }
    }
}
