using Demo.WebApi.Application.Public.Notifications;
using Demo.WebApi.Shared.Notifications;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NotificationFunction.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationFunction.Functions;

public class FCMNotificationQueueTrigger
{
    private readonly FCMSettings _config;

    public FCMNotificationQueueTrigger(IOptions<FCMSettings> options)
    {
        _config = options.Value;
    }

    [FunctionName(nameof(FCMNotificationQueueTrigger))]
    public async Task RunAsync(
        [QueueTrigger(QueueConstants.FCMQueueTrigger)]
        FCMNotificationRequest request,
        ILogger log)
    {
        var defaultApp = FirebaseApp.DefaultInstance;

        if (FirebaseApp.DefaultInstance == null)
        {
            defaultApp = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromJson(JsonConvert.SerializeObject(_config))
            });
        }

        var messaging = FirebaseMessaging.DefaultInstance;

        if (request.IsBatch)
        {
            await SendBatchNotifications(request, messaging, log);
        }
        else
        {
            await SendSingleNotification(request, messaging, log);
        }
    }

    private async Task SendBatchNotifications(FCMNotificationRequest request, FirebaseMessaging messaging, ILogger log)
    {
        int batchSize = 500;

        for (int i = 0; i < request.Tokens!.Count; i += batchSize)
        {
            List<string?> currentBatch = request.Tokens.Skip(i).Take(batchSize).Select(t => t.Value).ToList();

            if (currentBatch.Count == 0)
                return;

            var message = new MulticastMessage()
            {
                Notification = new Notification
                {
                    Title = request.Title,
                    Body = request.Body,
                    ImageUrl = request.ImageURL
                },
                Tokens = currentBatch,
                Data = request.Data
            };

            var response = await messaging.SendMulticastAsync(message);
            log.LogInformation($"FCM batch notification response: {response}");
        }
    }

    private async Task SendSingleNotification(FCMNotificationRequest request, FirebaseMessaging messaging, ILogger log)
    {
        var message = new Message()
        {
            Notification = new Notification
            {
                Title = request.Title,
                Body = request.Body,
                ImageUrl = request.ImageURL,
            },
            Data = request.Data
        };

        if (!string.IsNullOrWhiteSpace(request.Token!.Value))
        {
            message.Token = request.Token.Value;
        }
        else if (!string.IsNullOrWhiteSpace(request.Topic))
        {
            message.Topic = request.Topic;
        }
        else
        {
            message.Topic = "all";
        }

        var result = await messaging.SendAsync(message);
        log.LogInformation($"FCM queue trigger response: {result}");
    }
}
