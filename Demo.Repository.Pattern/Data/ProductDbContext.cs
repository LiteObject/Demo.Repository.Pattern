using Demo.Repository.Pattern.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Demo.Repository.Pattern.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext()
        {
            Environment.SpecialFolder specialFolder = Environment.SpecialFolder.LocalApplicationData;
            string path = Environment.GetFolderPath(specialFolder);
            DbPath = Path.Join(path, "DemoRepoPattern.db");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDbContext"/> class.
        /// </summary>
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the products.
        /// </summary>
        public DbSet<Product> Products { get; set; }

        public string DbPath
        {
            get;
        }

        /// <summary>
        /// The on configuring.
        /// </summary>
        /// <param name="optionsBuilder">
        /// The options builder.
        /// </param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /* Migrations with Multiple Providers: *****************************************************************
             * https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/providers?tabs=dotnet-core-cli
             * Two options to work with multiple providers:
             * 1. Using multiple context types
             * 2. Using one context type
             *******************************************************************************************************/

            ArgumentNullException.ThrowIfNull(optionsBuilder, nameof(optionsBuilder));

            // base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
                /* optionsBuilder.UseSqlServer(
                    "Server=(localdb)\\mssqllocaldb;Database=DemoEfCoreWithPostgres;Trusted_Connection=True;MultipleActiveResultSets=true",
                    option => { option.EnableRetryOnFailure(); }); */

                /* _ = optionsBuilder.UseNpgsql("Host=localhost;Database=postgres;Username=appuser;Password=Demo.01", builder.EnableRetryOnFailure())*/

                _ = optionsBuilder
                    .UseSqlite($"Data Source={DbPath}")
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging()
                    .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
            }
        }

        /// <summary>
        /// The on model creating.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*modelBuilder.Entity<Product>().Property<DateTime?>(p => p.CreatedOn).HasColumnType("timestamp without time zone");
            modelBuilder.Entity<Product>().Property<DateTime?>(p => p.UpdatedOn).HasColumnType("timestamp without time zone"); */

            _ = modelBuilder.Entity<Product>().HasData(
                new { Id = 1, Name = "Product One 1", UnitPrice = 1.5, CreatedOn = DateTime.UtcNow },
                new { Id = 2, Name = "Product One 2", UnitPrice = 0.5, CreatedOn = DateTime.UtcNow },
                new { Id = 3, Name = "Product Two", UnitPrice = 2.5, CreatedOn = DateTime.UtcNow },
                new
                {
                    Id = 4,
                    Name = "Old Product",
                    UnitPrice = 3.55,
                    CreatedOn = DateTime.UtcNow.AddDays(-150),
                    UpdatedOn = DateTime.UtcNow
                },
                new
                {
                    Id = 5,
                    Name = "Expensive Product",
                    UnitPrice = 150.99,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                }
            );

            _ = modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}