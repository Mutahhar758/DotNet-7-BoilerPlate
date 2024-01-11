namespace Demo.WebApi.Application.Preference.Cities;
public interface ICityService : IScopedService
{
    Task<List<LookupResponse>> GetLookupAsync(int? stateId, CancellationToken cancellationToken);
    Task<string> AddCityAsync(AddCityRequest request);
    Task<string> UpdateCityAsync(UpdateCityRequest request);
    Task<string> DeleteCityAsync(int id);
    Task<PaginationResponse<CityListingResponse>> GetAllCitiesAsync(CityListingRequest request, CancellationToken cancellationToken);
}
