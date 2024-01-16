using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.WebApi.Domain.Identity;
public class UserSession : AuditableEntity, IAggregateRoot
{
    public string Token { get; set; } = string.Empty;
    [ForeignKey(nameof(User))]
    public string UserId { get; set; }
    public string? DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public string? FcmToken { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string? UserRole { get; set; }
    public virtual ApplicationUser? User { get; set; }
}
