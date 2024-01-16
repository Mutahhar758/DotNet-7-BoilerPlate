namespace Demo.WebApi.Application.Preference.Countries;
public class CountryListingResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public DateTime? CreatedOn { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public List<ActionResponse> Actions { get; set; } = new();
}
