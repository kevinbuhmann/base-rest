using BaseRest.Boundary;
using BaseRest.Extensions;
using BaseRest.General;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Net;

namespace BaseRest.Queryable
{
    public sealed class QueryProvider<TDmn, TDto, TConverter, TPermissions> : IQueryProvider
        where TDmn : class, IDomain
        where TDto : class, IDto
        where TConverter : IConverter<TDmn, TDto, TPermissions>, new()
        where TPermissions : IPermissions
    {
        private readonly TPermissions permissions;
        private readonly HttpStatusCode getAllPermissions;

        private IQueryable<TDmn> internalQuery;
        private bool isGetAll;

        private List<IFilter<TDmn, TPermissions>> filters;
        private List<string> includes;

        internal QueryProvider(IQueryable<TDmn> internalQuery, TPermissions permissions, HttpStatusCode getAllPermissions, bool isGetAll)
        {
            internalQuery.ValidateNotNullParameter(nameof(internalQuery));
            permissions.ValidateNotNullParameter(nameof(permissions));

            this.permissions = permissions;
            this.getAllPermissions = getAllPermissions;

            this.filters = new List<IFilter<TDmn, TPermissions>>();
            this.includes = new List<string>();
            this.isGetAll = isGetAll;

            this.internalQuery = internalQuery;
        }

        public void Filter(IFilter<TDmn, TPermissions> filter)
        {
            HttpStatusCode filterPermissions = filter.HasPermissions(this.permissions);
            if (filterPermissions != HttpStatusCode.OK)
            {
                throw new RestfulException(filterPermissions);
            }

            this.isGetAll = false;
            this.internalQuery = filter.Apply(this.internalQuery);
        }

        public void OrderBy(string ordering)
        {
            this.internalQuery = this.internalQuery.OrderBy(ordering);
        }

        public void Include(string path)
        {
            this.includes.Add(path);
            this.internalQuery = this.internalQuery.Include(path);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new Queryable<TDmn, TDto, TConverter, TPermissions>(this, expression);
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
            if (this.getAllPermissions != HttpStatusCode.OK && this.isGetAll)
            {
                throw new RestfulException(this.getAllPermissions);
            }

            TDmn[] domains = this.internalQuery.ToArray();

            TConverter converter = new TConverter();
            string[] includes = this.includes.ToArray();

            IQueryable<TDto> dtos = domains
                .Select(dmn => converter.Convert(dmn, this.permissions, includes))
                .ToArray()
                .AsQueryable();

            ExpressionTreeModifier treeCopier = new ExpressionTreeModifier(dtos);
            Expression newExpressionTree = treeCopier.Visit(expression);

            return isEnumerable ?
                dtos.Provider.CreateQuery(newExpressionTree) :
                dtos.Provider.Execute(newExpressionTree);
        }
    }
}