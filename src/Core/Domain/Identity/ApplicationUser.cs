using Microsoft.AspNetCore.Identity;

namespace Demo.WebApi.Domain.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool AllowNotification { get; set; } = true;
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public string? ObjectId { get; set; }
    public virtual ICollection<UserSession>? UserSessions { get; set; }
}