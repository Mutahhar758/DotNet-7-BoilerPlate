using Ardalis.Specification.EntityFrameworkCore;
using Demo.WebApi.Application.Common.Enums;
using Demo.WebApi.Application.Common.Exceptions;
using Demo.WebApi.Application.Common.Interfaces;
using Demo.WebApi.Application.Common.Models;
using Demo.WebApi.Application.Common.Persistence;
using Demo.WebApi.Application.Preference.States;
using Demo.WebApi.Domain.Preference;
using Demo.WebApi.Infrastructure.Common.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Demo.WebApi.Infrastructure.Preference;
public class StateService : IStateService
{
    private readonly IRepository<State> _stateRepository;
    private readonly IStringLocalizer<StateService> _localizer;
    private readonly ICurrentUser _currentUser;

    public StateService(IRepository<State> stateRepository, IStringLocalizer<StateService> localizer, ICurrentUser currentUser)
    {
        _stateRepository = stateRepository;
        _localizer = localizer;
        _currentUser = currentUser;
    }

    public async Task<List<LookupResponse>> GetLookupAsync(int? countryId, CancellationToken cancellationToken)
    {
        return await _stateRepository
            .GetAll()
            .Where(c => countryId == null || c.CountryId == countryId)
            .ProjectToType<LookupResponse>()
            .ToListAsync(cancellationToken);
    }

    public async Task<string> AddStateAsync(AddStateRequest request)
    {
        if (await _stateRepository.GetAll().AnyAsync(m => m.Name.ToLower() == request.Name.ToLower()))
        {
            throw new ConflictException(string.Format(_localizer["State already exist."]));
        }

        var state = request.Adapt<State>();

        await _stateRepository.AddAsync(state);

        return _localizer["State has been added."];
    }

    public async Task<string> UpdateStateAsync(UpdateStateRequest request)
    {
        if (await _stateRepository.GetAll().AnyAsync(m => m.Id != request.Id && m.Name.ToLower() == request.Name.ToLower()))
        {
            throw new ConflictException(string.Format(_localizer["State already exist."]));
        }

        var state = await _stateRepository.GetAll().Where(m => m.Id == request.Id).FirstOrDefaultAsync();

        _ = state ?? throw new NotFoundException(_localizer["State Not Found."]);

        request.Adapt(state);

        await _stateRepository.UpdateAsync(state);

        return _localizer["State has been updated."];
    }

    public async Task<string> DeleteStateAsync(int id)
    {
        var state = await _stateRepository.GetAll().Where(m => m.Id == id).FirstOrDefaultAsync();

        _ = state ?? throw new NotFoundException(_localizer["State Not Found."]);

        await _stateRepository.DeleteAsync(state);

        return _localizer["State has been deleted."];
    }

    public async Task<PaginationResponse<StateListingResponse>> GetAllStatesAsync(StateListingRequest request, CancellationToken cancellationToken)
    {
        var statesQuery = _stateRepository.GetAll();

        var response = await statesQuery
            .Select(m => new StateListingResponse
            {
                Id = m.Id,
                Country = new LookupResponse { Id = m.Country!.Id, Name = m.Country.Name },
                Name = m.Name,
                CreatedOn = m.CreatedOn,
                LastModifiedOn = m.LastModifiedOn
            })
            .PaginatedListAsync(request);

        foreach (var state in response.Data)
        {
            state.Actions.Add(ActionType.View.ToActionResponse());

            state.Actions.Add(ActionType.Edit.ToActionResponse());

            state.Actions.Add(ActionType.Delete.ToActionResponse());
        }

        return response;
    }
}
