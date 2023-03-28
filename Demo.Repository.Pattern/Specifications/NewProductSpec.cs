using Demo.Repository.Pattern.Domain;
using System.Linq.Expressions;

namespace Demo.Repository.Pattern.Specifications
{
    public class NewProductSpec : BaseSpecification<Product>
    {
        public override Expression<Func<Product, bool>> ToExpression()
        {
            return product => product.CreatedOn > DateTime.Today.AddDays(-14);
        }
    }
}
