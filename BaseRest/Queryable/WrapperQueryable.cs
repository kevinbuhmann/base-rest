using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BaseRest.Queryable
{
    public class WrapperQueryable<T> : IOrderedQueryable<T>
    {
        internal WrapperQueryable(IQueryProvider provider, Expression expression)
        {
            this.Provider = provider;
            this.Expression = expression;
        }

        public Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        public Expression Expression { get; }

        public IQueryProvider Provider { get; }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Provider.Execute<IEnumerable<T>>(this.Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Provider.Execute<IEnumerable>(this.Expression).GetEnumerator();
        }
    }
}
