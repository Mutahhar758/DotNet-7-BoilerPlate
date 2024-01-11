namespace Demo.WebApi.Application.Preference.States;
public class UpdateStateRequest
{
    public int Id { get; set; }

    public int CountryId { get; set; }

    public string Name { get; set; } = default!;
}
