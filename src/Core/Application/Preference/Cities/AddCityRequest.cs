namespace Demo.WebApi.Application.Preference.Cities;
public class AddCityRequest
{
    public int StateId { get; set; }

    public string Name { get; set; } = default!;
}
