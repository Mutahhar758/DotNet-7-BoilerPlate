namespace Demo.WebApi.Domain.Preference;
public class State : AuditableEntity, IAggregateRoot
{
    public string Name { get; set; } = default!;

    public int CountryId { get; set; }

    public virtual Country? Country { get; set; }

    public virtual ICollection<City>? Cities { get; set; }
}
