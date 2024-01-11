namespace Demo.WebApi.Application.Preference.States;
public interface IStateService : IScopedService
{
    Task<List<LookupResponse>> GetLookupAsync(int? countryId, CancellationToken cancellationToken);
    Task<string> AddStateAsync(AddStateRequest request);
    Task<string> UpdateStateAsync(UpdateStateRequest request);
    Task<string> DeleteStateAsync(int id);
    Task<PaginationResponse<StateListingResponse>> GetAllStatesAsync(StateListingRequest request, CancellationToken cancellationToken);
}
