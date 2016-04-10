using BaseRest.Boundary;
using BaseRest.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BaseRest.Service.Converters
{
    public abstract class BaseConverter<TDmn, TDto, TPermissions> : IConverter<TDmn, TDto, TPermissions>
        where TDmn : class, IDomain
        where TDto : class, IDto
        where TPermissions : IPermissions
    {
        public abstract TDto Convert(TDmn domain, TPermissions permissions, string[] includes, DeletedState deletedState);

        protected TProp HandlePermissions<TProp>(
            bool hasPermissions,
            List<string> excludedProperties,
            TDmn domain,
            string[] includes,
            Expression<Func<TDmn, TProp>> dmnExpression,
            Expression<Func<TDto, TProp>> dtoExpression,
            bool autoInclude)
        {
            if (!hasPermissions)
            {
                excludedProperties.Add(GetMemberName(dtoExpression));
            }

            return hasPermissions && (autoInclude || HasInclude(dtoExpression, includes)) ? ExecuteMember(dmnExpression, domain) : default(TProp);
        }

        protected TDtoProp HandlePermissions<TDmnProp, TDtoProp>(
            bool hasPermissions,
            TPermissions permissions,
            List<string> excludedProperties,
            TDmn domain,
            string[] includes,
            DeletedState deletedState,
            Expression<Func<TDmn, TDmnProp>> dmnExpression,
            Expression<Func<TDto, TDtoProp>> dtoExpression,
            IConverter<TDmnProp, TDtoProp, TPermissions> converter)
            where TDmnProp : class, IDomain
            where TDtoProp : class, IDto
        {
            if (!hasPermissions)
            {
                excludedProperties.Add(GetMemberName(dtoExpression));
            }

            return hasPermissions && HasInclude(dtoExpression, includes) ?
                ExecuteMemberAndConvert(permissions, domain, includes, deletedState, dmnExpression, dtoExpression, converter) : default(TDtoProp);
        }

        protected TDtoProp[] HandlePermissions<TDmnProp, TDtoProp>(
            bool hasPermissions,
            TPermissions permissions,
            List<string> excludedProperties,
            TDmn domain,
            string[] includes,
            DeletedState deletedState,
            Expression<Func<TDmn, IEnumerable<TDmnProp>>> dmnExpression,
            Expression<Func<TDto, TDtoProp[]>> dtoExpression,
            IConverter<TDmnProp, TDtoProp, TPermissions> converter)
            where TDmnProp : class, IDomain
            where TDtoProp : class, IDto
        {
            if (!hasPermissions)
            {
                excludedProperties.Add(GetMemberName(dtoExpression));
            }

            return hasPermissions && HasInclude(dtoExpression, includes) ?
                ExecuteMemberAndConvert(permissions, domain, includes, deletedState, dmnExpression, dtoExpression, converter) : null;
        }

        private static TDtoProp ExecuteMemberAndConvert<TDmnProp, TDtoProp>(
            TPermissions permissions,
            TDmn domain,
            string[] includes,
            DeletedState deletedState,
            Expression<Func<TDmn, TDmnProp>> dmnExpression,
            Expression<Func<TDto, TDtoProp>> dtoExpression,
            IConverter<TDmnProp, TDtoProp, TPermissions> converter)
            where TDmnProp : class, IDomain
            where TDtoProp : class, IDto
        {
            string dtoMemberName = GetMemberName(dtoExpression).ToLower();
            string[] nextIncludes = GetNextIncludes(includes, dtoMemberName);

            TDmnProp domainProperty = ExecuteMember(dmnExpression, domain);

            return deletedState == DeletedState.All || domainProperty.UtcDateDeleted.HasValue == (deletedState == DeletedState.Deleted) ?
                converter.Convert(domainProperty, permissions, nextIncludes, deletedState) : null;
        }

        private static TDtoProp[] ExecuteMemberAndConvert<TDmnProp, TDtoProp>(
            TPermissions permissions,
            TDmn domain,
            string[] includes,
            DeletedState deletedState,
            Expression<Func<TDmn, IEnumerable<TDmnProp>>> dmnExpression,
            Expression<Func<TDto, TDtoProp[]>> dtoExpression,
            IConverter<TDmnProp, TDtoProp, TPermissions> converter)
            where TDmnProp : class, IDomain
            where TDtoProp : class, IDto
        {
            string dtoMemberName = GetMemberName(dtoExpression).ToLower();
            string[] nextIncludes = GetNextIncludes(includes, dtoMemberName);

            TDmnProp[] domainProperty = ExecuteMember(dmnExpression, domain)
                .Where(dmn => deletedState == DeletedState.All || dmn.UtcDateDeleted.HasValue == (deletedState == DeletedState.Deleted))
                .ToArray();

            return domainProperty
                .Select(i => converter.Convert(i, permissions, nextIncludes, deletedState))
                .ToArray();
        }

        private static TProp ExecuteMember<TProp>(Expression<Func<TDmn, TProp>> expression, TDmn instance)
        {
            return expression.Compile().Invoke(instance);
        }

        private static string GetMemberName<T, U>(Expression<Func<T, U>> expression)
        {
            return (expression.Body as MemberExpression).Member.Name;
        }

        private static bool HasInclude<TProp>(Expression<Func<TDto, TProp>> dtoExpression, string[] includes)
        {
            string dtoMemberName = GetMemberName(dtoExpression).ToLower();
            return GetCurrentIncludes(includes).Contains(dtoMemberName);
        }

        private static string[] GetCurrentIncludes(string[] includes)
        {
            return (includes ?? new string[0])
                .Select(i => i.Split('.').First())
                .ToArray();
        }

        private static string[] GetNextIncludes(string[] includes, string property)
        {
            string s = property + ".";
            return (includes ?? new string[0])
                .Where(i => i.StartsWith(s) && i.Length > s.Length)
                .Select(i => i.Substring(s.Length))
                .ToArray();
        }
    }
}
