using BaseRest.Boundary;
using BaseRest.Extensions;
using BaseRest.General;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Net;

namespace BaseRest.Queryable
{
    public sealed class Queryable<TDmn, TDto, TConverter, TPermissions> : IOrderedQueryable<TDto>
        where TDmn : class, IDomain
        where TDto : class, IDto
        where TConverter : IConverter<TDmn, TDto, TPermissions>, new()
        where TPermissions : IPermissions
    {
        private bool optionsApplied = false;

        public Type ElementType
        {
            get
            {
                return typeof(TDto);
            }
        }

        public IQueryProvider Provider { get; }

        public Expression Expression { get; private set; }

        public Queryable(IQueryable<TDmn> internalQuery, TPermissions permissions, HttpStatusCode getAllPermissions, bool isGetAll)
        {
            internalQuery.ValidateNotNullParameter(nameof(internalQuery));
            permissions.ValidateNotNullParameter(nameof(permissions));

            this.Provider = new QueryProvider<TDmn, TDto, TConverter, TPermissions>(internalQuery, permissions, getAllPermissions, isGetAll);
            this.Expression = Expression.Constant(this);
        }

        internal Queryable(IQueryProvider provider, Expression expression)
        {
            this.Provider = provider;
            this.Expression = expression;
        }

        public Queryable<TDmn, TDto, TConverter, TPermissions> Filter(IFilter<TDmn, TPermissions> filter)
        {
            filter.ValidateNotNullParameter(nameof(filter));

            (this.Provider as QueryProvider<TDmn, TDto, TConverter, TPermissions>).Filter(filter);
            return this;
        }

        public Queryable<TDmn, TDto, TConverter, TPermissions> Include(string path)
        {
            path.ValidateNotNullParameter(nameof(path));

            (this.Provider as QueryProvider<TDmn, TDto, TConverter, TPermissions>).Include(path);
            return this;
        }

        public Queryable<TDmn, TDto, TConverter, TPermissions> WithOptions(QueryOptions<TDmn, TDto, TPermissions> options)
        {
            options.ValidateNotNullParameter(nameof(options));

            if (this.optionsApplied)
            {
                throw new Exception("options alread applied");
            }

            // domain options (modifiy internal query)
            foreach (IFilter<TDmn, TPermissions> filter in options.Filters ?? new IFilter<TDmn, TPermissions>[0])
            {
                this.Filter(filter);
            }

            foreach (string include in options.Includes ?? new string[0])
            {
                this.Include(include);
            }

            // dto options (modify expression)
            string ordering = string.Join(",", (options.OrderBy ?? new string[0])
                .Select(i => i.StartsWith("-") ? $"{i.Substring(1)} descending" : i));
            if (!string.IsNullOrEmpty(ordering))
            {
                this.Expression = this.OrderBy(ordering).Expression;
            }

            if (options.Skip.HasValue)
            {
                this.Expression = this.Skip(options.Skip.Value).Expression;
            }

            if (options.Take.HasValue)
            {
                this.Expression = this.Take(options.Take.Value).Expression;
            }

            return this;
        }

        public IEnumerator<TDto> GetEnumerator()
        {
            return this.Provider.Execute<IEnumerable<TDto>>(this.Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Provider.Execute<IEnumerable>(this.Expression).GetEnumerator();
        }
    }
}
