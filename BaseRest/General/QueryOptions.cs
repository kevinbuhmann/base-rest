using BaseRest.Boundary;
using BaseRest.Extensions;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace BaseRest.General
{
    public sealed class QueryOptions<TDmn, TDto, TPermissions>
        where TDmn : class, IDomain
        where TDto : class, IDto
        where TPermissions : IPermissions
    {
        public string[] Includes { get; }

        public IFilter<TDmn, TPermissions>[] Filters { get; }

        private QueryOptions(string[] includes, IFilter<TDmn, TPermissions>[] filters)
        {
            this.Includes = includes;
            this.Filters = filters;
        }

        public static QueryOptions<TDmn, TDto, TPermissions> FromRequestUri(Uri requestUri)
        {
            string queryString = requestUri.Query.ToLower();
            NameValueCollection query = HttpUtility.ParseQueryString(queryString);

            // ?include=postalcode,funds
            string include = query["include"];
            string[] includes = !string.IsNullOrEmpty(include) ?
                include.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : null;

            // ?filter=bypostalcodenumber:63755|filtername:param1_param2_arrayvalue1,arrayvalue2
            string filter = query["filter"];
            string[] filterStrings = !string.IsNullOrEmpty(filter) ?
                filter.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) : null;
            IFilter<TDmn, TPermissions>[] filters = filterStrings?
                .Select(filterString => GetFilter(filterString)).ToArray();

            return new QueryOptions<TDmn, TDto, TPermissions>(includes, filters);
        }

        private static IFilter<TDmn, TPermissions> GetFilter(string filterString)
        {
            Regex pattern = new Regex("^([a-z]+):([0-9a-z_, ]+)+$");
            Match match = pattern.Match(filterString);

            if (!match.Success)
            {
                throw new RestfulException(HttpStatusCode.BadRequest);
            }

            string filterName = match.Groups[1].Value;
            string[] filterParams = Regex.Split(match.Groups[2].Value ?? string.Empty, @"(?<!\\)_");

            Type filterType = typeof(TDto).Assembly.GetTypes()
                .Where(t => t.Name.ToLower() == $"{filterName}filter" && typeof(IFilter<TDmn, TPermissions>).IsAssignableFrom(t))
                .FirstOrDefault();

            if (filterType == null)
            {
                throw new RestfulException(HttpStatusCode.BadRequest);
            }

            return filterType.InstantiateFromStringArray(filterParams) as IFilter<TDmn, TPermissions>;
        }
    }
}
