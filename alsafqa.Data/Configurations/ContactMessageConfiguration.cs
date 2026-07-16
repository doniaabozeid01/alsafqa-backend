using alsafqa.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace alsafqa.Data.Configurations;

public class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
{
    public void Configure(EntityTypeBuilder<ContactMessage> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.FullName).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Email).IsRequired().HasMaxLength(256);
        builder.Property(m => m.Phone).IsRequired().HasMaxLength(30);
        builder.Property(m => m.Subject).IsRequired().HasMaxLength(300);
        builder.Property(m => m.Message).IsRequired().HasMaxLength(4000);

        builder.HasIndex(m => m.IsRead);
        builder.HasIndex(m => m.CreatedAt);

        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}
