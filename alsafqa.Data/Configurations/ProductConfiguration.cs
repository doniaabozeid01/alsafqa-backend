using alsafqa.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace alsafqa.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.NameAr).IsRequired().HasMaxLength(300);
        builder.Property(p => p.NameEn).IsRequired().HasMaxLength(300);
        builder.Property(p => p.DescriptionAr).HasMaxLength(1000);
        builder.Property(p => p.DescriptionEn).HasMaxLength(1000);
        builder.Property(p => p.ImageUrl).HasMaxLength(1000);
        builder.Property(p => p.WeightInGrams).HasPrecision(10, 2);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
