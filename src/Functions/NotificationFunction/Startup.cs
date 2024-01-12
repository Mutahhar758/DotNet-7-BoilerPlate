using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationFunction;
using NotificationFunction.Models;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace NotificationFunction;
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        IConfiguration localConfig = new ConfigurationBuilder()
         .SetBasePath(Environment.CurrentDirectory)
         .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
       .AddEnvironmentVariables()
       .Build();

        builder.Services
            .Configure<FCMSettings>(localConfig.GetSection("FCMSettings"));

        builder.Services
            .Configure<MessageSettings>(localConfig.GetSection("MessageSettings"));

        builder.Services
            .Configure<MailSettings>(localConfig.GetSection("MailSettings"));
    }
}