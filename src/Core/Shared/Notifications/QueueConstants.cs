using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.WebApi.Shared.Notifications;
public static class QueueConstants
{
    public const string FCMQueueTrigger = "send-fcm-notification-queue";
    public const string SMSQueueTrigger = "send-sms-queue";
    public const string MailQueueTrigger = "send-mail-queue";
}
