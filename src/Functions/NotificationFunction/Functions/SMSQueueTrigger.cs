using Demo.WebApi.Application.Common.Mesasging;
using Demo.WebApi.Shared.Notifications;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NotificationFunction.Models;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace NotificationFunction.Functions;

public class SMSQueueTrigger
{
    private readonly MessageSettings _settings;
    public SMSQueueTrigger(IOptions<MessageSettings> settings)
    {
        _settings = settings.Value;
    }

    [FunctionName(nameof(SMSQueueTrigger))]
    public async Task RunAsync([QueueTrigger(QueueConstants.SMSQueueTrigger)] MessageRequest request, ILogger logger)
    {
        string? fromNumber = _settings.From;
        TwilioClient.Init(_settings.AccountSID, _settings.AuthToken);

        var messageOptions = new CreateMessageOptions(new PhoneNumber(request.To));
        messageOptions.From = new PhoneNumber(fromNumber);
        messageOptions.Body = request.Body;

        var response = await MessageResource.CreateAsync(messageOptions);
        var message = await MessageResource.FetchAsync(pathSid: response.Sid);

        if (message.Status == MessageResource.StatusEnum.Failed || message.Status == MessageResource.StatusEnum.Undelivered)
        {
            logger.LogError($"Message failed to deliver to {request.To} due to {message.ErrorMessage}.");
            throw new Exception($"Message failed to deliver to {request.To} due to {message.ErrorMessage}.");
        }

        logger.LogInformation($"SMS Queue trigger function result: {JsonConvert.SerializeObject(message.Status)}");
    }
}
