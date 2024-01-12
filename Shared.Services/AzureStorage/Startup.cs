using Demo.WebApi.Application.Storage;
using Microsoft.Extensions.DependencyInjection;
using Shared.Services.Common;

namespace Demo.WebApi.Shared.Services.AzureStorage;
public static class Startup
{
    public static IServiceCollection AddAzureQueues(this IServiceCollection services) =>
        services.AddService(typeof(IStorageQueueClient<>), typeof(AzureQueueClient<>), ServiceLifetime.Scoped);
}
