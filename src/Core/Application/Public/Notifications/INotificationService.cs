using Demo.WebApi.Application.Notification;

namespace Demo.WebApi.Application.Notifications;
public interface INotificationService : IScopedService
{
    Task<int> GetUnseenNotificationAsync();
    Task<PaginationResponse<NotificationListingResponse>> ListAsync(PaginationFilter request, CancellationToken cancellationToken);
    Task<string> ToggleStatusAsync(bool allowNotification);
    Task SendBatchNotifications(BatchNotificationRequest request);
    Task<int> SaveNotificationAsync(Demo.WebApi.Domain.Public.Notification notification);
    Task SaveUserNotificationAsync(SaveUserNotificationRequest request);
}