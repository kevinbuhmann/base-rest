using System;
using System.Collections.Specialized;
using System.Web;

namespace BaseRest.General
{
    public class QueryOptions
    {
        public string[] Includes { get; }

        public QueryOptions(string[] includes)
        {
            this.Includes = includes;
        }

        public static QueryOptions FromRequestUri(Uri requestUri)
        {
            string queryString = requestUri.Query.ToLower();
            NameValueCollection query = HttpUtility.ParseQueryString(queryString);

            string[] includes = !string.IsNullOrEmpty(query["include"]) ?
                query["include"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : null;

            return new QueryOptions(includes);
        }
    }
}
