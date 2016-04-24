using System;

namespace BaseRest.Extensions
{
    public static class ValidateExtensions
    {
        public static void ValidateNotNullParameter(this object parameter, string parameterName)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}
