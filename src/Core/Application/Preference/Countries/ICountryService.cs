namespace Demo.WebApi.Application.Preference.Countries;
public interface ICountryService : IScopedService
{
    Task<List<LookupResponse>> GetLookupAsync(CancellationToken cancellationToken);
    Task<string> AddCountryAsync(AddCountryRequest request);
    Task<string> UpdateCountryAsync(UpdateCountryRequest request);
    Task<string> DeleteCountryAsync(int id);
    Task<PaginationResponse<CountryListingResponse>> GetAllCountriesAsync(CountryListingRequest request, CancellationToken cancellationToken);
}
