using Demo.WebApi.Application.Common.Models;

namespace Demo.WebApi.Application.Preference.States;
public class StateListingResponse
{
    public int Id { get; set; }

    public LookupResponse? Country { get; set; }

    public string Name { get; set; } = default!;

    public DateTime? CreatedOn { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public List<ActionResponse> Actions { get; set; } = new();
}
