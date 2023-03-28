using Demo.Repository.Pattern.Domain;
using System.Linq.Expressions;

namespace Demo.Repository.Pattern.Specifications
{
    internal class ExpensiveProductSpec : BaseSpecification<Product>
    {
        public override Expression<Func<Product, bool>> ToExpression()
        {
            return product => product.UnitPrice > 100;
        }
    }
}
