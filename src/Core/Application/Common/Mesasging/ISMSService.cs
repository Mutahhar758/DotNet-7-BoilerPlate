using Demo.WebApi.Application.Common.Mesasging;

namespace Demo.WebApi.Application.Common.Messaging;

public interface ISMSService : ITransientService
{
    Task SendAsync(MessageRequest request);
}