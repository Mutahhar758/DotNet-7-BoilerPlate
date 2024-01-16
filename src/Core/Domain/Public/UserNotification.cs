using Demo.WebApi.Domain.Common.Enums;

namespace Demo.WebApi.Domain.Public;
public class UserNotification : AuditableEntity, IAggregateRoot
{
    public string? UserId { get; set; }
    public string? Role { get; set; }
    public int NotificationId { get; set; }
    public NotificationStatus Status { get; set; } = default!;
    public virtual Notification? Notification { get; set; }
}
