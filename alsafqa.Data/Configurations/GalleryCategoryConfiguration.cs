using alsafqa.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace alsafqa.Data.Configurations;

public class GalleryCategoryConfiguration : IEntityTypeConfiguration<GalleryCategory>
{
    public void Configure(EntityTypeBuilder<GalleryCategory> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.NameAr).IsRequired().HasMaxLength(200);
        builder.Property(c => c.NameEn).IsRequired().HasMaxLength(200);

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasMany(c => c.Galleries)
            .WithOne(g => g.GalleryCategory)
            .HasForeignKey(g => g.GalleryCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
