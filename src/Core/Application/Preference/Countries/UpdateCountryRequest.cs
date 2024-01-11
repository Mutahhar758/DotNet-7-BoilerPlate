namespace Demo.WebApi.Application.Preference.Countries;
public class UpdateCountryRequest
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;
}
