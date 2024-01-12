using Demo.WebApi.Application.Preference.Cities;
using Demo.WebApi.Infrastructure.Common.Extensions;
using Microsoft.Extensions.Localization;

namespace Demo.WebApi.Host.WebAPI.Controllers.Preference;

[Authorize]
//[MustHavePermission(AppAction.View, AppResource.City)]
public sealed class CityController : VersionNeutralApiController
{
    private readonly ICityService _cityService;
    private readonly IStringLocalizer<CityController> _localizer;

    public CityController(ICityService cityService, IStringLocalizer<CityController> localizer)
    {
        _cityService = cityService;
        _localizer = localizer;
    }

    [HttpGet("lookup")]
    [OpenApiOperation("Get a lookup list of all cities.", "")]
    public async Task<DataResponseDetail<List<LookupResponse>>> GetCitiesLookupAsync(int? stateId, CancellationToken cancellationToken)
    {
        return (await _cityService.GetLookupAsync(stateId, cancellationToken))
            .ToDataResponse()
            .ToInformationResponse();
    }

    [HttpGet]
    [OpenApiOperation("Get a list of all cities.", "")]
    public async Task<PaginationResponse<CityListingResponse>> GetAllCitiesAsync([FromQuery] CityListingRequest request, CancellationToken cancellationToken)
    {
        return (await _cityService.GetAllCitiesAsync(request, cancellationToken))
            .ToInformationResponse();
    }

    [HttpPost]
    //[MustHavePermission(AppAction.Create, AppResource.City)]
    [OpenApiOperation("Add a city.", "")]
    public async Task<HttpResponseDetail> AddCityAsync(AddCityRequest request)
    {
        string msg = await _cityService.AddCityAsync(request);
        return HttpResponseExtension.SuccessResponse(msg);
    }

    [HttpPut]
    //[MustHavePermission(AppAction.Update, AppResource.City)]
    [OpenApiOperation("Update a City.", "")]
    public async Task<HttpResponseDetail> UpdateCityAsync(UpdateCityRequest request)
    {
        string msg = await _cityService.UpdateCityAsync(request);
        return HttpResponseExtension.SuccessResponse(msg);
    }

    [HttpDelete("{id}")]
    //[MustHavePermission(AppAction.Delete, AppResource.City)]
    [OpenApiOperation("Delete a City By Id.", "")]
    public async Task<HttpResponseDetail> DeleteCityAsync(int id)
    {
        string msg = await _cityService.DeleteCityAsync(id);
        return HttpResponseExtension.SuccessResponse(msg);
    }
}