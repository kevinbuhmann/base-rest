using System.Linq;
using System.Linq.Expressions;

namespace BaseRest.Queryable
{
    internal class ExpressionTreeModifier : ExpressionVisitor
    {
        private readonly IQueryable all;

        internal ExpressionTreeModifier(IQueryable all)
        {
            this.all = all;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            return (c.Type.IsGenericType && c.Type.GetGenericTypeDefinition() == typeof(Queryable<,,,>)) ?
                Expression.Constant(this.all) : c;
        }
    }
}
