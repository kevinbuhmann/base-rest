using BaseRest.Boundary;
using BaseRest.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace BaseRest.Queryable
{
    public class QueryProvider<TDmn, TDto, TConverter, TPermissions> : IQueryProvider
        where TDmn : class, IDomain
        where TDto : class, IDto
        where TConverter : IConverter<TDmn, TDto, TPermissions>, new()
        where TPermissions : IPermissions
    {
        private readonly TPermissions permissions;

        private List<string> includes;

        internal QueryProvider(IQueryable<TDmn> internalQuery, TPermissions permissions)
        {
            internalQuery.ValidateNotNullParameter(nameof(internalQuery));
            permissions.ValidateNotNullParameter(nameof(permissions));

            this.permissions = permissions;
            this.includes = new List<string>();

            this.InternalQuery = internalQuery;
        }

        public void Include(string path)
        {
            this.includes.Add(path);
            this.InternalQuery.Include(path);
        }

        public IQueryable<TDmn> InternalQuery { get; }

        public IQueryable CreateQuery(Expression expression)
        {
            return (IQueryable)Activator.CreateInstance(typeof(Queryable<,,,>).MakeGenericType(typeof(TDto), typeof(TDmn), typeof(TConverter), typeof(TPermissions)), new object[] { this, expression });
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) != typeof(TDto))
            {
                return new WrapperQueryable<TElement>(this, expression);
            }

            return new Queryable<TDmn, TDto, TConverter, TPermissions>(this, expression) as IQueryable<TElement>;
        }

        public object Execute(Expression expression)
        {
            return this.Execute(expression, false);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            bool isEnumerable = typeof(IEnumerable).IsAssignableFrom(typeof(TResult));
            return (TResult)this.Execute(expression, isEnumerable);
        }

        private object Execute(Expression expression, bool isEnumerable)
        {
            TConverter converter = new TConverter();
            string[] includes = this.includes.ToArray();

            TDmn[] domains = this.InternalQuery.ToArray();

            IQueryable<TDto> dtos = domains
                .Select(dmn => converter.Convert(dmn, this.permissions, includes))
                .ToArray()
                .AsQueryable();

            var treeCopier = new ExpressionTreeModifier(dtos);
            Expression newExpressionTree = treeCopier.Visit(expression);

            return isEnumerable ?
                dtos.Provider.CreateQuery(newExpressionTree) :
                dtos.Provider.Execute(newExpressionTree);
        }
    }
}