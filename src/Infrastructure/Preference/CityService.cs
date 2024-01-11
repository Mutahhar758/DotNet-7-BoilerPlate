using Ardalis.Specification.EntityFrameworkCore;
using Demo.WebApi.Application.Common.Enums;
using Demo.WebApi.Application.Common.Exceptions;
using Demo.WebApi.Application.Common.Interfaces;
using Demo.WebApi.Application.Common.Models;
using Demo.WebApi.Application.Common.Persistence;
using Demo.WebApi.Application.Preference.Cities;
using Demo.WebApi.Domain.Preference;
using Demo.WebApi.Infrastructure.Common.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Demo.WebApi.Infrastructure.Preference;
public class CityService : ICityService
{
    private readonly IRepository<City> _cityRepository;
    private readonly IStringLocalizer<CityService> _localizer;
    private readonly ICurrentUser _currentUser;

    public CityService(IRepository<City> cityRepository, IStringLocalizer<CityService> localizer, ICurrentUser currentUser)
    {
        _cityRepository = cityRepository;
        _localizer = localizer;
        _currentUser = currentUser;
    }

    public async Task<List<LookupResponse>> GetLookupAsync(int? stateId, CancellationToken cancellationToken)
    {
        return await _cityRepository
            .GetAll()
            .Where(c => stateId == null || c.StateId == stateId)
            .ProjectToType<LookupResponse>()
            .ToListAsync(cancellationToken);
    }

    public async Task<string> AddCityAsync(AddCityRequest request)
    {
        if (await _cityRepository.GetAll().AnyAsync(m => m.Name.ToLower() == request.Name.ToLower()))
        {
            throw new ConflictException(string.Format(_localizer["City already exist."]));
        }

        var city = request.Adapt<City>();

        await _cityRepository.AddAsync(city);

        return _localizer["City has been added."];
    }

    public async Task<string> UpdateCityAsync(UpdateCityRequest request)
    {
        if (await _cityRepository.GetAll().AnyAsync(m => m.Id != request.Id && m.Name.ToLower() == request.Name.ToLower()))
        {
            throw new ConflictException(string.Format(_localizer["City already exist."]));
        }

        var city = await _cityRepository.GetAll().Where(m => m.Id == request.Id).FirstOrDefaultAsync();

        _ = city ?? throw new NotFoundException(_localizer["City Not Found."]);

        request.Adapt(city);

        await _cityRepository.UpdateAsync(city);

        return _localizer["City has been updated."];
    }

    public async Task<string> DeleteCityAsync(int id)
    {
        var city = await _cityRepository.GetAll().Where(m => m.Id == id).FirstOrDefaultAsync();

        _ = city ?? throw new NotFoundException(_localizer["City Not Found."]);

        await _cityRepository.DeleteAsync(city);

        return _localizer["City has been deleted."];
    }

    public async Task<PaginationResponse<CityListingResponse>> GetAllCitiesAsync(CityListingRequest request, CancellationToken cancellationToken)
    {
        var citiesQuery = _cityRepository.GetAll();

        var response = await citiesQuery
            .Select(m => new CityListingResponse
            {
                Id = m.Id,
                Country = new LookupResponse { Id = m.State!.Country!.Id, Name = m.State.Country.Name },
                State = new LookupResponse { Id = m.State!.Id, Name = m.State.Name },
                Name = m.Name,
                CreatedOn = m.CreatedOn,
                LastModifiedOn = m.LastModifiedOn
            })
            .PaginatedListAsync(request);

        foreach (var city in response.Data)
        {
            city.Actions.Add(ActionType.View.ToActionResponse());

            city.Actions.Add(ActionType.Edit.ToActionResponse());

            city.Actions.Add(ActionType.Delete.ToActionResponse());
        }

        return response;
    }
}
