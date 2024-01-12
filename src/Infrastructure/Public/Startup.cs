using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using YDrive.Shared.Services.SignalR;

namespace YDrive.Infrastructure.Notifications;

public static class Startup
{
    public static IServiceCollection AddNotifications(this IServiceCollection services, IConfiguration config)
    {
        ILogger logger = Log.ForContext(typeof(Startup));

        services.Configure<SignalRSettings>(config.GetSection(nameof(SignalRSettings)));
        //var signalRSettings = config.GetSection(nameof(SignalRSettings)).Get<SignalRSettings>();
        return services;
    }
}