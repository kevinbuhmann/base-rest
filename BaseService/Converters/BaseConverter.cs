using BaseDomain.General;
using BaseService.General;
using General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;

namespace BaseService.Converters
{
    public abstract class BaseConverter<TDmn, TDto, TPermissions> : IConverter<TDmn, TDto, TPermissions>
        where TDmn : IDomain
        where TDto : IDto
        where TPermissions : IPermissions
    {
        public abstract TDto Convert(TDmn domain, TPermissions permissions, string[] includes);

        public abstract TDmn Create(TDto dto, TPermissions permissions);

        public virtual void Update(TDmn domain, TDto dto, TPermissions permissions)
        {
            if (domain.Id == 0)
            {
                throw new RestfulException(HttpStatusCode.Conflict, "Cannot update new domain.");
            }

            if (domain.Id != dto.Id)
            {
                throw new RestfulException(HttpStatusCode.Conflict, "Domain and Dto must have the same id.");
            }
        }

        protected TProp HandlePermissions<TProp>(
            bool hasPermissions,
            TDto dto,
            TDmn domain,
            string[] includes,
            Expression<Func<TDmn, TProp>> dmnExpression,
            Expression<Func<TDto, TProp>> dtoExpression,
            bool autoInclude)
        {
            if (!hasPermissions)
            {
                dto.ExcludedProperties.Add(GetMemberName(dtoExpression));
            }

            return hasPermissions && (autoInclude || HasInclude(dtoExpression, includes)) ? ExecuteMember(dmnExpression, domain) : default(TProp);
        }

        protected TDtoProp HandlePermissions<TDmnProp, TDtoProp>(
            bool hasPermissions,
            TPermissions permissions,
            TDto dto,
            TDmn domain,
            string[] includes,
            Expression<Func<TDmn, TDmnProp>> dmnExpression,
            Expression<Func<TDto, TDtoProp>> dtoExpression,
            IConverter<TDmnProp, TDtoProp, TPermissions> converter)
            where TDmnProp : IDomain
            where TDtoProp : IDto
        {
            if (!hasPermissions)
            {
                dto.ExcludedProperties.Add(GetMemberName(dtoExpression));
            }

            return hasPermissions && HasInclude(dtoExpression, includes) ?
                ExecuteMemberAndConvert(permissions, domain, includes, dmnExpression, dtoExpression, converter) : default(TDtoProp);
        }

        private static string GetMemberName<T, U>(Expression<Func<T, U>> expression)
        {
            return (expression.Body as MemberExpression).Member.Name;
        }

        private static TProp ExecuteMember<TProp>(Expression<Func<TDmn, TProp>> expression, TDmn instance)
        {
            return expression.Compile().Invoke(instance);
        }

        private static TDtoProp ExecuteMemberAndConvert<TDmnProp, TDtoProp>(
            TPermissions permissions,
            TDmn domain,
            string[] includes,
            Expression<Func<TDmn, TDmnProp>> dmnExpression,
            Expression<Func<TDto, TDtoProp>> dtoExpression,
            IConverter<TDmnProp, TDtoProp, TPermissions> converter)
            where TDmnProp : IDomain
            where TDtoProp : IDto
        {
            return converter.Convert(ExecuteMember(dmnExpression, domain), permissions, GetNextIncludes(includes, GetMemberName(dtoExpression)));
        }

        private static bool HasInclude<TProp>(Expression<Func<TDto, TProp>> dtoExpression, string[] includes)
        {
            string name = GetMemberName(dtoExpression).ToLower();

            return GetCurrentIncludes(includes).Contains(name);
        }

        private static string[] GetCurrentIncludes(string[] includes)
        {
            return includes
                .Select(i => i.Split('.').First())
                .ToArray();
        }

        private static string[] GetNextIncludes(string[] includes, string property)
        {
            string s = property + ".";
            return includes
                .Where(i => i.StartsWith(s) && i.Length > s.Length)
                .Select(i => i.Substring(s.Length))
                .ToArray();
        }
    }
}
