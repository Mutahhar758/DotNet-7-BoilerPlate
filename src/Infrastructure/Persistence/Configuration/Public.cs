using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Demo.WebApi.Domain.Public;

namespace Demo.WebApi.Infrastructure.Persistence.Configuration;
public class MediaConfig : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder
            .ToTable("Media", SchemaNames.Public);

        builder
            .Property(m => m.FileType)
                .HasMaxLength(256);
    }
}
