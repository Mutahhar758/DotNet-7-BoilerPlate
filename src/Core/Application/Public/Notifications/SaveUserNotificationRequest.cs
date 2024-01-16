namespace Demo.WebApi.Application.Public.Notifications;
public class SaveUserNotificationRequest
{
    public int NotificationId { get; set; }
    public List<FCMToken> Tokens { get; set; } = new List<FCMToken>();
}
