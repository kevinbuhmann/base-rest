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
        public IFilter<TDmn, TPermissions>[] Filters { get; }

        public string[] Includes { get; }

        public string[] OrderBy { get; }

        public int? Skip { get; }

        public int? Take { get; }

        public QueryOptions(IFilter<TDmn, TPermissions>[] filters, string[] includes, string[] orderBy, int? skip, int? take)
        {
            this.Filters = filters;
            this.Includes = includes;
            this.OrderBy = orderBy;
            this.Skip = skip;
            this.Take = take;
        }

        public static QueryOptions<TDmn, TDto, TPermissions> FromRequestUri(Uri requestUri)
        {
            string queryString = requestUri.Query.ToLower();
            NameValueCollection query = HttpUtility.ParseQueryString(queryString);

            // ?filter=bypostalcodenumber:63755|filtername:param1_param2_arrayvalue1,arrayvalue2
            string filterList = query["filter"];
            string[] filterStrings = !string.IsNullOrEmpty(filterList) ?
                filterList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) : null;
            IFilter<TDmn, TPermissions>[] filters = filterStrings?
                .Select(filterString => GetFilter(filterString)).ToArray();

            // ?include=postalcode,funds
            string includeList = query["include"];
            string[] includes = !string.IsNullOrEmpty(includeList) ?
                includeList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : null;

            // ?order-by=name,-id
            string orderByList = query["order-by"];
            string[] orderBy = !string.IsNullOrEmpty(orderByList) ?
                orderByList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : null;

            // ?skip=5
            string skipNumber = query["skip"];
            int? skip = !string.IsNullOrEmpty(skipNumber) ? skipNumber.ToIntOrNull() : null;

            // ?take=5
            string takeNumber = query["take"];
            int? take = !string.IsNullOrEmpty(takeNumber) ? takeNumber.ToIntOrNull() : null;

            return new QueryOptions<TDmn, TDto, TPermissions>(filters, includes, orderBy, skip, take);
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
