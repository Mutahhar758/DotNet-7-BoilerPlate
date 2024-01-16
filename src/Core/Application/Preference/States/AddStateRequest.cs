namespace Demo.WebApi.Application.Preference.States;
public class AddStateRequest
{
    public int CountryId { get; set; }

    public string Name { get; set; } = default!;
}
