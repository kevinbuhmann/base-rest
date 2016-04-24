using System.Linq;
using System.Linq.Expressions;

namespace BaseRest.Queryable
{
    internal class ExpressionTreeModifier : ExpressionVisitor
    {
        private readonly IQueryable queryable;

        internal ExpressionTreeModifier(IQueryable queryable)
        {
            this.queryable = queryable;
        }

        protected override Expression VisitConstant(ConstantExpression constant)
        {
            return (constant.Type.IsGenericType && constant.Type.GetGenericTypeDefinition() == typeof(Queryable<,,,>)) ?
                Expression.Constant(this.queryable) : constant;
        }
    }
}
