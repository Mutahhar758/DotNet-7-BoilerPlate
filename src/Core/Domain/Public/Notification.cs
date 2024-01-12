using Demo.WebApi.Domain.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.WebApi.Domain.Public;
public class Notification : AuditableEntity, IAggregateRoot
{
    public string Title { get; set; } = default!;

    public string Description { get; set; } = default!;

    public NotificationStatus Status { get; set; } = default!;

    [ForeignKey(nameof(Image))]
    public int? ImageId { get; set; }

    public string? Url { get; set; }

    [Column(TypeName = "jsonb")]
    public object? Arguments { get; set; }

    public virtual Media? Image { get; set; }
}
