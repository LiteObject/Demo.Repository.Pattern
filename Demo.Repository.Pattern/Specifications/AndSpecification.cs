using System.Linq.Expressions;

namespace Demo.Repository.Pattern.Specifications
{
    public class AndSpecification<T> : BaseSpecification<T>
    {
        private readonly BaseSpecification<T> _left;

        private readonly BaseSpecification<T> _right;

        public AndSpecification(BaseSpecification<T> left, BaseSpecification<T> right)
        {
            this._right = right;
            this._left = left;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            Expression<Func<T, bool>> leftExpression = this._left.ToExpression();
            Expression<Func<T, bool>> rightExpression = this._right.ToExpression();

            ParameterExpression paramExpression = Expression.Parameter(typeof(T));
            BinaryExpression? expressionBody = Expression.AndAlso(leftExpression.Body, rightExpression.Body);
            expressionBody = (BinaryExpression)new ParameterReplacer(paramExpression).Visit(expressionBody);
            Expression<Func<T, bool>> finalExpression = Expression.Lambda<Func<T, bool>>(expressionBody ?? throw new InvalidOperationException($"{nameof(expressionBody)} cannot be null."), paramExpression);

            return finalExpression;
        }
    }
}
