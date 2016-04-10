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
        public Type ElementType
        {
            get
            {
                return typeof(TDto);
            }
        }

        public IQueryProvider Provider { get; }

        public Expression Expression { get; }

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
            (this.Provider as QueryProvider<TDmn, TDto, TConverter, TPermissions>).Filter(filter);
            return this;
        }

        public Queryable<TDmn, TDto, TConverter, TPermissions> OrderBy(string ordering)
        {
            (this.Provider as QueryProvider<TDmn, TDto, TConverter, TPermissions>).OrderBy(ordering);
            return this;
        }

        public Queryable<TDmn, TDto, TConverter, TPermissions> OrderBy<TKey>(Expression<Func<TDmn, TKey>> keySelector)
        {
            (this.Provider as QueryProvider<TDmn, TDto, TConverter, TPermissions>).OrderBy(keySelector);
            return this;
        }

        public Queryable<TDmn, TDto, TConverter, TPermissions> OrderBy<TKey>(Expression<Func<TDmn, TKey>> keySelector, IComparer<TKey> comparer)
        {
            (this.Provider as QueryProvider<TDmn, TDto, TConverter, TPermissions>).OrderBy(keySelector, comparer);
            return this;
        }

        public Queryable<TDmn, TDto, TConverter, TPermissions> OrderByDescending<TKey>(Expression<Func<TDmn, TKey>> keySelector)
        {
            (this.Provider as QueryProvider<TDmn, TDto, TConverter, TPermissions>).OrderByDescending(keySelector);
            return this;
        }

        public Queryable<TDmn, TDto, TConverter, TPermissions> OrderByDescending<TKey>(Expression<Func<TDmn, TKey>> keySelector, IComparer<TKey> comparer)
        {
            (this.Provider as QueryProvider<TDmn, TDto, TConverter, TPermissions>).OrderByDescending(keySelector, comparer);
            return this;
        }

        public Queryable<TDmn, TDto, TConverter, TPermissions> ThenBy<TKey>(Expression<Func<TDmn, TKey>> keySelector)
        {
            (this.Provider as QueryProvider<TDmn, TDto, TConverter, TPermissions>).ThenBy(keySelector);
            return this;
        }

        public Queryable<TDmn, TDto, TConverter, TPermissions> ThenBy<TKey>(Expression<Func<TDmn, TKey>> keySelector, IComparer<TKey> comparer)
        {
            (this.Provider as QueryProvider<TDmn, TDto, TConverter, TPermissions>).ThenBy(keySelector, comparer);
            return this;
        }

        public Queryable<TDmn, TDto, TConverter, TPermissions> ThenByDescending<TKey>(Expression<Func<TDmn, TKey>> keySelector)
        {
            (this.Provider as QueryProvider<TDmn, TDto, TConverter, TPermissions>).ThenByDescending(keySelector);
            return this;
        }

        public Queryable<TDmn, TDto, TConverter, TPermissions> ThenByDescending<TKey>(Expression<Func<TDmn, TKey>> keySelector, IComparer<TKey> comparer)
        {
            (this.Provider as QueryProvider<TDmn, TDto, TConverter, TPermissions>).ThenByDescending(keySelector, comparer);
            return this;
        }

        public Queryable<TDmn, TDto, TConverter, TPermissions> Skip(int count)
        {
            (this.Provider as QueryProvider<TDmn, TDto, TConverter, TPermissions>).Skip(count);
            return this;
        }

        public Queryable<TDmn, TDto, TConverter, TPermissions> Take(int count)
        {
            (this.Provider as QueryProvider<TDmn, TDto, TConverter, TPermissions>).Take(count);
            return this;
        }

        public Queryable<TDmn, TDto, TConverter, TPermissions> Include(string path)
        {
            (this.Provider as QueryProvider<TDmn, TDto, TConverter, TPermissions>).Include(path);
            return this;
        }

        public Queryable<TDmn, TDto, TConverter, TPermissions> ApplyOptions(QueryOptions<TDmn, TDto, TPermissions> options)
        {
            options.ValidateNotNullParameter(nameof(options));

            foreach (IFilter<TDmn, TPermissions> filter in options.Filters ?? new IFilter<TDmn, TPermissions>[0])
            {
                this.Filter(filter);
            }

            string ordering = string.Join(",", (options.OrderBy ?? new string[0])
                .Select(i => i.StartsWith("-") ? $"{i.Substring(1)} descending" : i));
            if (!string.IsNullOrEmpty(ordering))
            {
                this.OrderBy(ordering);
            }

            if (options.Skip.HasValue)
            {
                this.Skip(options.Skip.Value);
            }

            if (options.Take.HasValue)
            {
                this.Take(options.Take.Value);
            }

            foreach (string include in options.Includes ?? new string[0])
            {
                this.Include(include);
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
