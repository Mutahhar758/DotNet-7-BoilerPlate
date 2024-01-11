namespace Demo.WebApi.Domain.Preference;
public class Country : AuditableEntity, IAggregateRoot
{
    public string Name { get; set; } = default!;

    public virtual ICollection<State>? States { get; set; }
}
