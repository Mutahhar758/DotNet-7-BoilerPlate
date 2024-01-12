using Demo.WebApi.Application.Preference.Countries;
using Demo.WebApi.Infrastructure.Common.Extensions;
using Microsoft.Extensions.Localization;

namespace Demo.WebApi.Host.Controllers.Preference;

[Authorize]
//[MustHavePermission(AppAction.View, AppResource.Country)]
public sealed class CountryController : VersionNeutralApiController
{
    private readonly ICountryService _countryService;
    private readonly IStringLocalizer<CountryController> _localizer;

    public CountryController(ICountryService countryService, IStringLocalizer<CountryController> localizer)
    {
        _countryService = countryService;
        _localizer = localizer;
    }

    [HttpGet("lookup")]
    [OpenApiOperation("Get a lookup list of all countries.", "")]
    public async Task<DataResponseDetail<List<LookupResponse>>> GetCountriesLookupAsync(CancellationToken cancellationToken)
    {
        return (await _countryService.GetLookupAsync(cancellationToken))
            .ToDataResponse()
            .ToInformationResponse();
    }

    [HttpGet]
    [OpenApiOperation("Get a list of all countries.", "")]
    public async Task<PaginationResponse<CountryListingResponse>> GetAllCountriesAsync([FromQuery] CountryListingRequest request, CancellationToken cancellationToken)
    {
        return (await _countryService.GetAllCountriesAsync(request, cancellationToken))
            .ToInformationResponse();
    }

    [HttpPost]
    //[MustHavePermission(AppAction.Create, AppResource.Country)]
    [OpenApiOperation("Add a country.", "")]
    public async Task<HttpResponseDetail> AddCountryAsync(AddCountryRequest request)
    {
        string msg = await _countryService.AddCountryAsync(request);
        return HttpResponseExtension.SuccessResponse(msg);
    }

    [HttpPut]
    //[MustHavePermission(AppAction.Update, AppResource.Country)]
    [OpenApiOperation("Update a Country.", "")]
    public async Task<HttpResponseDetail> UpdateCountryAsync(UpdateCountryRequest request)
    {
        string msg = await _countryService.UpdateCountryAsync(request);
        return HttpResponseExtension.SuccessResponse(msg);
    }

    [HttpDelete("{id}")]
    //[MustHavePermission(AppAction.Delete, AppResource.Country)]
    [OpenApiOperation("Delete a Country By Id.", "")]
    public async Task<HttpResponseDetail> DeleteCountryAsync(int id)
    {
        string msg = await _countryService.DeleteCountryAsync(id);
        return HttpResponseExtension.SuccessResponse(msg);
    }
}