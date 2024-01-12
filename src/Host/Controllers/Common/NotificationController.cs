using Demo.WebApi.Application.Public.Notifications;
using Demo.WebApi.Infrastructure.Common.Extensions;

namespace Demo.WebApi.Host.Controllers.Common;

[Authorize()]
public class NotificationController : VersionNeutralApiController
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPatch("update-status")]
    [OpenApiOperation("change status", "")]
    public async Task<HttpResponseDetail> ToggleStatusAsync([FromBody]bool allowNotification, CancellationToken cancellationToken)
    {
        string message = await _notificationService.ToggleStatusAsync(allowNotification);
        return HttpResponseExtension.SuccessResponse(message);
    }

    [HttpGet]
    [OpenApiOperation("Get Notification list", "")]
    public async Task<PaginationResponse<NotificationListingResponse>> ListAsync([FromQuery]PaginationFilter request, CancellationToken cancellationToken)
    {
        return (await _notificationService.ListAsync(request, cancellationToken)).ToInformationResponse();
    }

    [HttpGet("unseen-count")]
    [OpenApiOperation("Get Unseen Notification count", "")]
    public async Task<DataResponseDetail<int>> GetUnseenNotificationAsync(CancellationToken cancellationToken)
    {
        return (await _notificationService.GetUnseenNotificationAsync()).ToDataResponse().ToInformationResponse();
    }

    [HttpPost("send-notification")]
    [OpenApiOperation("send fcm notification", "")]
    public async Task<HttpResponseDetail> SendNotification(FCMNotificationRequest request)
    {
        await _notificationService.SendAsync(request);
        return HttpResponseExtension.SuccessResponse().ToInformationResponse();
    }
}