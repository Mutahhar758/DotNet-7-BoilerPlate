using Demo.WebApi.Application.Common.Enums;
using Demo.WebApi.Application.Common.Models;

namespace Demo.WebApi.Infrastructure.Common.Extensions;

public static class ActionExtension
{
    public static ActionResponse ToActionResponse(this ActionType action)
    {
        return new ActionResponse { Name = action.GetDescription(), Code = (int)action };
    }
}
