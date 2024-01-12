using Demo.WebApi.Application.Common.Mailing;
using Demo.WebApi.Application.Storage;
using Demo.WebApi.Shared.Notifications;

namespace Demo.WebApi.Infrastructure.Mailing;

public class MailService : IMailService
{
    private readonly IStorageQueueClient<MailRequest> _queueClient;
    public MailService(IStorageQueueClient<MailRequest> queueClient)
    {
        _queueClient = queueClient;
    }

    public async Task SendAsync(MailRequest request)
    {
        await _queueClient.InsertAsync(request, QueueConstants.MailQueueTrigger);
    }
}