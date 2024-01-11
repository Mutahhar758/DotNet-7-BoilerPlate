namespace Demo.WebApi.Application.Preference.Cities;
public class UpdateCityRequest
{
    public int Id { get; set; }

    public int StateId { get; set; }

    public string Name { get; set; } = default!;
}
