using Demo.Repository.Pattern.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Repository.Pattern.Data.Configurations
{
    public class ProductConfiguration : BaseConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            _ = builder.Property(p => p.UnitPrice).HasDefaultValue(0.01d);
            base.Configure(builder);
        }
    }
}
