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

public class NotificationConfig : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder
            .ToTable("Notification", SchemaNames.Public);

    }
}

public class UserNotificationConfig : IEntityTypeConfiguration<UserNotification>
{
    public void Configure(EntityTypeBuilder<UserNotification> builder)
    {
        builder
            .ToTable("UserNotification", SchemaNames.Public);

    }
}
