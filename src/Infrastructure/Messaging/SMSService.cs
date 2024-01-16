using Demo.WebApi.Application.Common.Mesasging;
using Demo.WebApi.Application.Common.Messaging;
using Demo.WebApi.Application.Storage;
using Demo.WebApi.Shared.Notifications;

namespace Demo.WebApi.Infrastructure.Messaging;
public class SMSService : ISMSService
{
    private readonly IStorageQueueClient<MessageRequest> _queueClient;

    public SMSService(IStorageQueueClient<MessageRequest> queueClient)
    {
        _queueClient = queueClient;
    }

    public async Task SendAsync(MessageRequest request)
    {
        await _queueClient.InsertAsync(request, QueueConstants.SMSQueueTrigger);
    }
}