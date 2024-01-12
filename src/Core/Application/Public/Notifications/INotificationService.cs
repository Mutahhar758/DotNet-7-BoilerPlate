namespace Demo.WebApi.Application.Public.Notifications;
public interface INotificationService : IScopedService
{
    Task<int> GetUnseenNotificationAsync();
    Task<PaginationResponse<NotificationListingResponse>> ListAsync(PaginationFilter request, CancellationToken cancellationToken);
    Task<string> ToggleStatusAsync(bool allowNotification);
    Task<int> SaveNotificationAsync(Demo.WebApi.Domain.Public.Notification notification);
    Task SaveUserNotificationAsync(SaveUserNotificationRequest request);
    Task SendAsync(FCMNotificationRequest request);
}