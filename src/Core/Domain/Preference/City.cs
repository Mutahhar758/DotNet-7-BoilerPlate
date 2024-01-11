namespace Demo.WebApi.Domain.Preference;
public class City : AuditableEntity, IAggregateRoot
{
    public string Name { get; set; } = default!;

    public int StateId { get; set; }

    public virtual State? State { get; set; }
}
