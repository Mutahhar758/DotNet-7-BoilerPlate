using YDrive.Application.Common.FCMNotification;

namespace Demo.WebApi.Application.Notifications;
public class SaveUserNotificationRequest
{
    public int NotificationId { get; set; }
    public List<FCMToken> Tokens { get; set; } = new List<FCMToken>();
}
