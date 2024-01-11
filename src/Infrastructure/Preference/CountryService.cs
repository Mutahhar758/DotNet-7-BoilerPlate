using Ardalis.Specification.EntityFrameworkCore;
using Demo.WebApi.Application.Common.Enums;
using Demo.WebApi.Application.Common.Exceptions;
using Demo.WebApi.Application.Common.Interfaces;
using Demo.WebApi.Application.Common.Models;
using Demo.WebApi.Application.Common.Persistence;
using Demo.WebApi.Application.Preference.Countries;
using Demo.WebApi.Domain.Preference;
using Demo.WebApi.Infrastructure.Common.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Demo.WebApi.Infrastructure.Preference;
public class CountryService : ICountryService
{
    private readonly IRepository<Country> _countryRepository;
    private readonly IStringLocalizer<CountryService> _localizer;
    private readonly ICurrentUser _currentUser;

    public CountryService(IRepository<Country> countryRepository, IStringLocalizer<CountryService> localizer, ICurrentUser currentUser)
    {
        _countryRepository = countryRepository;
        _localizer = localizer;
        _currentUser = currentUser;
    }

    public async Task<List<LookupResponse>> GetLookupAsync(CancellationToken cancellationToken)
    {
        return await _countryRepository.GetAll<LookupResponse>().ToListAsync(cancellationToken);
    }

    public async Task<string> AddCountryAsync(AddCountryRequest request)
    {
        if (await _countryRepository.GetAll().AnyAsync(m => m.Name.ToLower() == request.Name.ToLower()))
        {
            throw new ConflictException(string.Format(_localizer["Country already exist."]));
        }

        var country = request.Adapt<Country>();

        await _countryRepository.AddAsync(country);

        return _localizer["Country has been added."];
    }

    public async Task<string> UpdateCountryAsync(UpdateCountryRequest request)
    {
        if (await _countryRepository.GetAll().AnyAsync(m => m.Id != request.Id && m.Name.ToLower() == request.Name.ToLower()))
        {
            throw new ConflictException(string.Format(_localizer["Country already exist."]));
        }

        var country = await _countryRepository.GetAll().Where(m => m.Id == request.Id).FirstOrDefaultAsync();

        _ = country ?? throw new NotFoundException(_localizer["Country Not Found."]);

        request.Adapt(country);

        await _countryRepository.UpdateAsync(country);

        return _localizer["Country has been updated."];
    }

    public async Task<string> DeleteCountryAsync(int id)
    {
        var country = await _countryRepository.GetAll().Where(m => m.Id == id).FirstOrDefaultAsync();

        _ = country ?? throw new NotFoundException(_localizer["Country Not Found."]);

        await _countryRepository.DeleteAsync(country);

        return _localizer["Country has been deleted."];
    }

    public async Task<PaginationResponse<CountryListingResponse>> GetAllCountriesAsync(CountryListingRequest request, CancellationToken cancellationToken)
    {
        var countriesQuery = _countryRepository.GetAll();

        var response = await countriesQuery
            .Select(m => new CountryListingResponse
            {
                Id = m.Id,
                Name = m.Name,
                CreatedOn = m.CreatedOn,
                LastModifiedOn = m.LastModifiedOn
            })
            .PaginatedListAsync(request);

        foreach (var country in response.Data)
        {
            country.Actions.Add(ActionType.View.ToActionResponse());

            country.Actions.Add(ActionType.Edit.ToActionResponse());

            country.Actions.Add(ActionType.Delete.ToActionResponse());
        }

        return response;
    }
}
