using Demo.WebApi.Domain.Common.Enums;

namespace Demo.WebApi.Application.Public.Notifications;
public class NotificationListingResponse
{
    public int Id { get; set; } = default!;

    public string Title { get; set; } = default!;

    public string Description { get; set; } = default!;

    public string? Url { get; set; }

    public string? ImageURL { get; set; }

    public DateTime CreatedOn { get; set; }

    public NotificationStatus Status { get; set; } = default!;
}
