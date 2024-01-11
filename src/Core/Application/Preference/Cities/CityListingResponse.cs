namespace Demo.WebApi.Application.Preference.Cities;
public class CityListingResponse
{
    public int Id { get; set; }

    public LookupResponse? Country { get; set; }

    public LookupResponse? State { get; set; }

    public string Name { get; set; } = default!;

    public DateTime? CreatedOn { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public List<ActionResponse> Actions { get; set; } = new();
}
