using System.Text.RegularExpressions;

namespace BaseRest.Extensions
{
    public static class StringExtensions
    {
        public static string CamelCaseToSplitLower(this string source)
        {
            return Regex.Replace(source, "([A-Z_])", "-$1", RegexOptions.Compiled).ToLower().Trim(new char[] { '-' });
        }
    }
}
