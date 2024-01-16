using Ardalis.Specification.EntityFrameworkCore;
using Demo.WebApi.Application.Common.Interfaces;
using Demo.WebApi.Application.Common.Models;
using Demo.WebApi.Application.Common.Persistence;
using Demo.WebApi.Application.Identity.Users;
using Demo.WebApi.Application.Public.Media;
using Demo.WebApi.Application.Public.Notifications;
using Demo.WebApi.Application.Storage;
using Demo.WebApi.Domain.Common.Enums;
using Demo.WebApi.Domain.Identity;
using Demo.WebApi.Domain.Public;
using Demo.WebApi.Infrastructure.Common.Extensions;
using Demo.WebApi.Shared.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace Demo.WebApi.Infrastructure.Public;
public class NotificationService : INotificationService
{
    private readonly IRepository<Notification> _notificationRepository;
    private readonly ICurrentUser _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<UserSession> _sessionRepository;
    private readonly IMediaService _documentService;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<NotificationService> _localizer;
    private readonly IRepository<UserNotification> _userNotificationRepository;
    private readonly ILogger<NotificationService> _logger;
    private readonly IStorageQueueClient<FCMNotificationRequest> _queueClient;

    public NotificationService(
        IRepository<Notification> notificationRepository,
        ICurrentUser currentUser,
        UserManager<ApplicationUser> userManager,
        IRepository<UserSession> sessionRepository,
        IMediaService documentService,
        IRepository<UserNotification> userNotificationRepository,
        IStringLocalizer<NotificationService> localizer,
        IUserService userService,
        ILogger<NotificationService> logger,
        IStorageQueueClient<FCMNotificationRequest> queueClient)
    {
        _notificationRepository = notificationRepository;
        _currentUser = currentUser;
        _userManager = userManager;
        _sessionRepository = sessionRepository;
        _documentService = documentService;
        _userNotificationRepository = userNotificationRepository;
        _localizer = localizer;
        _userService = userService;
        _logger = logger;
        _queueClient = queueClient;
    }

    public async Task<string> ToggleStatusAsync(bool allowNotification)
    {
        var user = await _userManager.FindByIdAsync(_currentUser.GetUserId().ToString());

        user.AllowNotification = allowNotification;
        await _userManager.UpdateAsync(user);
        return string.Format(_localizer["Notification preference has been updated."]);
    }

    public async Task<int> GetUnseenNotificationAsync()
    {
        return await _userNotificationRepository.GetAll()
            .Where(x => x.UserId == _currentUser.GetUserId().ToString() &&
                //x.Role == _currentUser.GetProfileIdType().Type &&
                x.Status == NotificationStatus.Unseen)
            .CountAsync();
    }

    public async Task<PaginationResponse<NotificationListingResponse>> ListAsync(PaginationFilter request, CancellationToken cancellationToken)
    {
        var notificationQuery = _userNotificationRepository.GetAll().Include(x => x.Notification)
            .Where(x => x.UserId == _currentUser.GetUserId().ToString() /*&& x.Role == _currentUser.GetProfileIdType().Type*/);

        var response = await notificationQuery
            .OrderByDescending(x => x.CreatedOn)
            .Select(x => new NotificationListingResponse
            {
                Id = x.Id,
                Title = x.Notification!.Title,
                Description = x.Notification.Description,
                ImageURL = x.Notification.Image!.AccessURL,
                Status = x.Status,
                CreatedOn = x.CreatedOn,
                Url = x.Notification.Url
            })
            .PaginatedListAsync(request);

        var records = await _userNotificationRepository
            .GetAll()
            .Where(x => x.UserId == _currentUser.GetUserId().ToString() &&
                //x.Role == _currentUser.GetProfileIdType().Type &&
                x.Status == NotificationStatus.Unseen)
            .ToListAsync();

        records.ForEach(x => x.Status = NotificationStatus.Seen);

        await _userNotificationRepository.UpdateRangeAsync(records);

        return response;
    }

    public async Task<int> SaveNotificationAsync(Notification notification)
    {
       return (await _notificationRepository.AddAsync(notification)).Id;
    }

    public async Task SaveUserNotificationAsync(SaveUserNotificationRequest request)
    {
        foreach(FCMToken user in request.Tokens)
        {
            await _userNotificationRepository.AddAsync(new UserNotification
            {
                NotificationId = request.NotificationId,
                UserId = user.UserId,
                Role = user.Role,
                Status = NotificationStatus.Unseen
            });
        }
    }

    public async Task SendAsync(FCMNotificationRequest request)
    {
        await _queueClient.InsertAsync(request, QueueConstants.FCMQueueTrigger, request.ScheduleAt);
    }
}
