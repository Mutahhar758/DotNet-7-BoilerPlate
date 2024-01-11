using Demo.WebApi.Infrastructure.Persistence.Context;
using Demo.WebApi.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Demo.WebApi.Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Demo.WebApi.Infrastructure.Identity;

internal static class Startup
{
    internal static IServiceCollection AddIdentity(this IServiceCollection services) =>
        services
            .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.User.RequireUniqueEmail = true;
                })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .Services;

    internal static IServiceCollection AddUserSessionMiddleware(this IServiceCollection services) =>
        services.AddScoped<UserSessionMiddleware>();

    internal static IApplicationBuilder UseUserSessionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<UserSessionMiddleware>();
    }
}