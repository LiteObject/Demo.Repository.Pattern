using System.Linq.Expressions;

namespace Demo.Repository.Pattern.Specifications
{
    internal class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression parameter;

        internal ParameterReplacer(ParameterExpression parameter)
        {
            this.parameter = parameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(this.parameter);
        }
    }
}
