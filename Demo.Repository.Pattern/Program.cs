using AutoMapper;
using Demo.Repository.Pattern.Data;
using Demo.Repository.Pattern.Domain;
using Demo.Repository.Pattern.Profiles;
using Demo.Repository.Pattern.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using System.Reflection;

namespace Demo.Repository.Pattern
{
    internal static class Program
    {
        private static readonly IConfiguration Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        private static readonly IMapper Mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductProfile>();
            // cfg.AddMaps(typeof(Program));
        }).CreateMapper();

        static Program()
        {
            Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        public static async Task Main()
        {
            string? connectionString = Configuration.GetConnectionString("DB_CONNECTION");

            //DbContextOptions<ProductDbContext> options = new DbContextOptionsBuilder<ProductDbContext>()
            //    .LogTo(m => Print(m, ConsoleColor.Yellow), Microsoft.Extensions.Logging.LogLevel.Information)
            //    .UseSqlite(connectionString)
            //    .EnableSensitiveDataLogging()
            //    .Options;

            await using ProductDbContext context = new();
            _ = await context.Database.EnsureDeletedAsync();
            _ = await context.Database.EnsureCreatedAsync();

            /*var p = new Product { 
                Id = 1, Name = "Product One", CreatedOn= DateTime.Now, CreatedBy = "mohammed"
            };
            p.CreatedBy = "mohammed"; */

            // Option 1:
            /*var products = await context.Products.ToListAsync();
            var productDtos = new List<ProductDto>();
            products.ForEach(p => productDtos.Add(Mapper.Map<ProductDto>(p)));
            productDtos.ForEach(p => Print(p.Name)); */

            // Option 2:
            /*var productDtos = await context.Products.ProjectTo<ProductDto>(Mapper.ConfigurationProvider).ToListAsync();
            productDtos.ForEach(p => Print(p.Name)); */

            // Option 3:
            using GenericRepository<Product, ProductDbContext> repo = new(context, Mapper);
            //var productDtos = await repo.FindAsync<ProductDto>(p => p.Name.Contains("One"), p => p.m);
            //productDtos.ForEach(p => Print(p.Name));

            NewProductSpec newProducts = new();
            ExpensiveProductSpec expProducts = new();

            // BaseSpecification<Product> combinedSpec = newProducts.And(expProducts);

            //var products = await repo.FindAsync(newProducts);
            Dictionary<string, object> dict = new()
            {
                { "Name", "Product One 1" },
                { "UnitPrice", 1.5 }
            };


            Expression<Func<Product, bool>> exp1 = p => p.Name.Contains("One") && p.UnitPrice > 1;
            Expression<Func<Product, bool>> exp2 = BuildExpression<Product>(dict);

            Func<Product, bool> func1 = BuildFunc<Product>(
                    p => p.Name.Contains("One"), p => p.UnitPrice > 1
                );

            Console.WriteLine(exp1.Body.ToString());
            Console.WriteLine(exp2.Body.ToString());

            List<Product> products = await repo.FindAsync(exp2);

            Console.WriteLine(products.Count);
            products.ForEach(p => Print(p.Name));
        }

        private static void Print(string value, ConsoleColor consoleColor = ConsoleColor.Cyan)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}]: {value}");
            Console.ResetColor();
        }

        private static Expression<Func<T, bool>> BuildExpression<T>(Dictionary<string, object> searchCriteria)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "p");
            List<Expression> expressions = new();


            foreach (KeyValuePair<string, object> kvp in searchCriteria)
            {
                PropertyInfo? propertyInfo = typeof(T).GetProperty(kvp.Key,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                object searchValue = kvp.Value;
                MemberExpression propertyExpression = Expression.Property(parameterExpression, propertyInfo);
                ConstantExpression constantExpression = Expression.Constant(searchValue);
                BinaryExpression equalityExpression = Expression.Equal(propertyExpression, constantExpression);

                expressions.Add(equalityExpression);
            }

            if (expressions.Count == 0)
            {
                return x => true;
            }

            Expression andExpression = expressions.Aggregate(Expression.AndAlso);

            return Expression.Lambda<Func<T, bool>>(andExpression, parameterExpression);
        }

        public static Func<T, bool> BuildFunc<T>(params Expression<Func<T, bool>>[] conditions)
        {
            ParameterExpression param = Expression.Parameter(typeof(T), "entity");
            Expression? body = null;

            foreach (Expression<Func<T, bool>> condition in conditions)
            {
                Expression conditionBody = condition.Body.ReplaceParameter(condition.Parameters[0], param);
                body = body == null ? conditionBody : Expression.AndAlso(body, conditionBody);
            }

            Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(body, param);
            return lambda.Compile();
        }

        public static Expression ReplaceParameter(this Expression expression, ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            return new ParameterReplacerVisitor(oldParameter, newParameter).Visit(expression);
        }
    }

    public class ParameterReplacerVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression oldParameter;
        private readonly ParameterExpression newParameter;

        public ParameterReplacerVisitor(
            ParameterExpression oldParameter,
            ParameterExpression newParameter)
        {
            this.oldParameter = oldParameter;
            this.newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == oldParameter ? newParameter : base.VisitParameter(node);
        }
    }
}