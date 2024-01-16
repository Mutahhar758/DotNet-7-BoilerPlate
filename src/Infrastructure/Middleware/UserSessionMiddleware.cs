using Demo.WebApi.Application.Common.Exceptions;
using Demo.WebApi.Application.Identity.Sessions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Demo.WebApi.Infrastructure.Middleware;
public class UserSessionMiddleware : IMiddleware
{
    private readonly ISessionService _sessionService;
    private readonly IStringLocalizer<UserSessionMiddleware> _localizer;

    public UserSessionMiddleware(ISessionService sessionService, IStringLocalizer<UserSessionMiddleware> localizer)
    {
        _sessionService = sessionService;
        _localizer = localizer;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string? token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
        {
            bool verified = await _sessionService.VerifyTokenSessionAsync(token!);

            if (!verified)
                throw new UnauthorizedException(_localizer["Oops! We couldn't verify your session, please try again."]);
        }

        await next(context);
    }
}
