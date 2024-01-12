using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YDrive.Application.Common.FCMNotification;
public interface IFCMNotificationService : ITransientService
{
    Task SendAsync(FCMNotificationRequest request);
}
