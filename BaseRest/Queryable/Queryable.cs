using BaseRest.Boundary;
using BaseRest.Extensions;
using BaseRest.General;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BaseRest.Queryable
{
    public class Queryable<TDmn, TDto, TConverter, TPermissions> : IOrderedQueryable<TDto>
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

        public Queryable(IQueryable<TDmn> internalQuery, TPermissions permissions)
        {
            internalQuery.ValidateNotNullParameter(nameof(internalQuery));
            permissions.ValidateNotNullParameter(nameof(permissions));

            this.Provider = new QueryProvider<TDmn, TDto, TConverter, TPermissions>(internalQuery, permissions);
            this.Expression = Expression.Constant(this);
        }

        public Queryable(IQueryProvider provider, Expression expression)
        {
            this.Provider = provider;
            this.Expression = expression;
        }

        public Queryable<TDmn, TDto, TConverter, TPermissions> Include(string path)
        {
            (this.Provider as QueryProvider<TDmn, TDto, TConverter, TPermissions>).Include(path);
            return this;
        }

        public Queryable<TDmn, TDto, TConverter, TPermissions> ApplyOptions(QueryOptions options)
        {
            options.ValidateNotNullParameter(nameof(options));

            if (options.Includes != null)
            {
                foreach (string include in options.Includes)
                {
                    this.Include(include);
                }
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
