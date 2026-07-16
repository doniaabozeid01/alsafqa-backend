using alsafqa.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace alsafqa.Data.Configurations;

public class GalleryConfiguration : IEntityTypeConfiguration<Gallery>
{
    public void Configure(EntityTypeBuilder<Gallery> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.TitleAr).IsRequired().HasMaxLength(300);
        builder.Property(g => g.TitleEn).IsRequired().HasMaxLength(300);
        builder.Property(g => g.DescriptionAr).HasMaxLength(1000);
        builder.Property(g => g.DescriptionEn).HasMaxLength(1000);
        builder.Property(g => g.ImageUrl).HasMaxLength(1000);

        builder.HasQueryFilter(g => !g.IsDeleted);
    }
}
