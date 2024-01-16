using Demo.WebApi.Application.Preference.States;
using Demo.WebApi.Infrastructure.Common.Extensions;
using Microsoft.Extensions.Localization;

namespace Demo.WebApi.Host.Controllers.Preference;

[Authorize]
//[MustHavePermission(AppAction.View, AppResource.State)]
public sealed class StateController : VersionNeutralApiController
{
    private readonly IStateService _stateService;
    private readonly IStringLocalizer<StateController> _localizer;

    public StateController(IStateService stateService, IStringLocalizer<StateController> localizer)
    {
        _stateService = stateService;
        _localizer = localizer;
    }

    [HttpGet("lookup")]
    [OpenApiOperation("Get a lookup list of all states.", "")]
    public async Task<DataResponseDetail<List<LookupResponse>>> GetStatesLookupAsync(int? countryId, CancellationToken cancellationToken)
    {
        return (await _stateService.GetLookupAsync(countryId, cancellationToken))
            .ToDataResponse()
            .ToInformationResponse();
    }

    [HttpGet]
    [OpenApiOperation("Get a list of all states.", "")]
    public async Task<PaginationResponse<StateListingResponse>> GetAllStatesAsync([FromQuery] StateListingRequest request, CancellationToken cancellationToken)
    {
        return (await _stateService.GetAllStatesAsync(request, cancellationToken))
            .ToInformationResponse();
    }

    [HttpPost]
    //[MustHavePermission(AppAction.Create, AppResource.State)]
    [OpenApiOperation("Add a state.", "")]
    public async Task<HttpResponseDetail> AddStateAsync(AddStateRequest request)
    {
        string msg = await _stateService.AddStateAsync(request);
        return HttpResponseExtension.SuccessResponse(msg);
    }

    [HttpPut]
    //[MustHavePermission(AppAction.Update, AppResource.State)]
    [OpenApiOperation("Update a State.", "")]
    public async Task<HttpResponseDetail> UpdateStateAsync(UpdateStateRequest request)
    {
        string msg = await _stateService.UpdateStateAsync(request);
        return HttpResponseExtension.SuccessResponse(msg);
    }

    [HttpDelete("{id}")]
    //[MustHavePermission(AppAction.Delete, AppResource.State)]
    [OpenApiOperation("Delete a State By Id.", "")]
    public async Task<HttpResponseDetail> DeleteStateAsync(int id)
    {
        string msg = await _stateService.DeleteStateAsync(id);
        return HttpResponseExtension.SuccessResponse(msg);
    }
}