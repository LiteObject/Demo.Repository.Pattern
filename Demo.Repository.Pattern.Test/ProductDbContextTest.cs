using Demo.Repository.Pattern.Data;
using Demo.Repository.Pattern.Domain;
using Microsoft.EntityFrameworkCore;

namespace Demo.Repository.Pattern.Test
{
    public class ProductDbContextTest : IDisposable
    {
        private readonly ProductDbContext context;

        private bool disposed;

        public ProductDbContextTest()
        {
            string databaseName = new Guid().ToString();
            DbContextOptions<ProductDbContext> options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName).Options;

            this.context = new ProductDbContext(options);

            //List<Product> testProductData = new() {
            //    new Product() { Id = 1, Name = "Product One", UnitPrice = 1.5, CreatedOn = DateTime.UtcNow },
            //    new  Product() { Id = 2, Name = "Product Two", UnitPrice = 2.5, CreatedOn = DateTime.UtcNow },
            //};

            //this.context.Products.AddRangeAsync(testProductData);

            //this.context.SaveChanges();
        }

        [Fact]
        public async Task ShouldReturnProductById()
        {
            // ARRANGE
            List<Product> testProductData = new() {
                new Product() { Id = 1, Name = "Product One", UnitPrice = 1.5, CreatedOn = DateTime.UtcNow },
                new  Product() { Id = 2, Name = "Product Two", UnitPrice = 2.5, CreatedOn = DateTime.UtcNow },
            };

            await context.Products.AddRangeAsync(testProductData);
            _ = await context.SaveChangesAsync();

            int productId = testProductData[0].Id;

            // ACT
            Product? product = await context.Products.FindAsync(productId);

            // ASSERT
            Assert.NotNull(product);
            Assert.Equal(productId, product.Id);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                // Free any other managed objects here.
                context.Dispose();
            }

            this.disposed = true;
        }
    }
}