using Ardalis.Specification.EntityFrameworkCore;
using Demo.WebApi.Application.Common.Interfaces;
using Demo.WebApi.Application.Common.Persistence;
using Demo.WebApi.Application.Notifications;
using Demo.WebApi.Domain.Public;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq.Dynamic.Core;

namespace YDrive.Infrastructure.Notifications;
public class NotificationService : INotificationService
{
    private readonly IRepository<Notification> _notificationRepository;
    private readonly ICurrentUser _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<UserSession> _sessionRepository;
    private readonly IFCMNotificationService _fcmNotificationService;
    private readonly IDocumentService _documentService;
    private readonly IAdminRolePermissionService _adminRolePermissionService;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<NotificationService> _localizer;
    private readonly IRepository<UserNotification> _userNotificationRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Driver> _driverRepository;
    private readonly IRepository<Corporate> _corporateRepository;

    public NotificationService(
        IRepository<Notification> notificationRepository,
        ICurrentUser currentUser,
        UserManager<ApplicationUser> userManager,
        IRepository<UserSession> sessionRepository,
        IFCMNotificationService fcmNotificationService,
        IDocumentService documentService,
        IAdminRolePermissionService adminRolePermissionService,
        IRepository<UserNotification> userNotificationRepository,
        IStringLocalizer<NotificationService> localizer,
        IUserService userService,
        IRepository<Domain.Profile.Corporate> corporateRepository,
        IRepository<Driver> driverRepository,
        IRepository<Customer> customerRepository)
    {
        _notificationRepository = notificationRepository;
        _currentUser = currentUser;
        _userManager = userManager;
        _sessionRepository = sessionRepository;
        _fcmNotificationService = fcmNotificationService;
        _documentService = documentService;
        _adminRolePermissionService = adminRolePermissionService;
        _userNotificationRepository = userNotificationRepository;
        _localizer = localizer;
        _userService = userService;
        _corporateRepository = corporateRepository;
        _driverRepository = driverRepository;
        _customerRepository = customerRepository;
    }

    public async Task<string> ToggleStatusAsync(bool allowNotification)
    {
        var user = await _userManager.FindByIdAsync(_currentUser.GetUserId().ToString());

        user.AllowNotification = allowNotification;
        await _userManager.UpdateAsync(user);
        return string.Format(_localizer[MessageConstants.RecordUpdated], "Notification preference");
    }

    public async Task<int> GetUnseenNotificationAsync()
    {
        return await _userNotificationRepository.GetAll()
            .Where(x => x.UserId == _currentUser.GetUserId().ToString() &&
                x.Role == _currentUser.GetProfileIdType().Type &&
                x.Status == NotificationStatus.Unseen)
            .CountAsync();
    }

    public async Task<PaginationResponse<NotificationListingResponse>> ListAsync(PaginationFilter request, CancellationToken cancellationToken)
    {
        var notificationQuery = _userNotificationRepository.GetAll().Include(x => x.Notification)
            .Where(x => x.UserId == _currentUser.GetUserId().ToString() && x.Role == _currentUser.GetProfileIdType().Type);

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
                x.Role == _currentUser.GetProfileIdType().Type &&
                x.Status == NotificationStatus.Unseen)
            .ToListAsync();

        records.ForEach(x => x.Status = NotificationStatus.Seen);

        await _userNotificationRepository.UpdateRangeAsync(records);

        return response;
    }

    public async Task SendBatchNotifications(BatchNotificationRequest request)
    {
        List<string> userIds = new();
        if (!request.AllCustomers && request.CustomerIds?.Count > 0)
        {
            userIds.AddRange(await _userService.GetUserIdsFromProfileAsync(AppRoles.Customer, request.CustomerIds));
        }

        if (!request.AllDrivers && request.DriverIds?.Count > 0)
        {
            userIds.AddRange(await _userService.GetUserIdsFromProfileAsync(AppRoles.Driver, request.DriverIds));
        }

        if (!request.AllCorporates && request.CorporateIds?.Count > 0)
        {
            userIds.AddRange(await _userService.GetUserIdsFromProfileAsync(AppRoles.Corporate, request.CorporateIds));
        }

        var tokensQuery = _sessionRepository
            .GetAll()
            .Where(s =>
                !string.IsNullOrWhiteSpace(s.FcmToken) &&
                s.User!.AllowNotification &&
                (
                    (request.AllCustomers && s.User!.ApplicationUserRoles!.Any(ur => ur.Role!.Name == AppRoles.Customer)) ||
                    (request.AllDrivers && s.User!.ApplicationUserRoles!.Any(ur => ur.Role!.Name == AppRoles.Driver)) ||
                    (request.AllCorporates && s.User!.ApplicationUserRoles!.Any(ur => ur.Role!.Name == AppRoles.Corporate)) ||
                    userIds.Contains(s.UserId)
                ));

        var tokens = await tokensQuery
            .Select(s => new FCMToken { Value = s.FcmToken, Role = s.UserRole, UserId = s.UserId})
            .Distinct()
            .ToListAsync();

        var document = new DocumentResponse();

        if (request.Image != null)
        {
            document = await _documentService.AddDocumentAsync(new DocumentRequest { DocumentFile = request.Image, Path = "public/notification" });
        }

        await _fcmNotificationService.SendAsync(new FCMNotificationRequest
        {
            Title = request.Title,
            Body = request.Body,
            Url = request.Url,
            ImageURL = document.AccessURL,
            Data = new Dictionary<string, string> { { "Target", NotificationTargetConstants.Notification }, { "Url", request.Url ?? string.Empty } },
            IsBatch = true,
            Tokens = tokens!
        });

        var notification = new Notification
        {
            Title = request.Title,
            Description = request.Body,
            Status = NotificationStatus.Unseen,
            ImageId = document.Id != 0 ? document.Id : null,
            Url = request.Url,
            CustomerIds = request.AllCustomers ? new List<int>() :
                request.CustomerIds?.Count > 0 ? request.CustomerIds : null,
            DriverIds = request.AllDrivers ? new List<int>() :
                request.DriverIds?.Count > 0 ? request.DriverIds : null,
            CorporateIds = request.AllCorporates ? new List<int>() :
                request.CorporateIds?.Count > 0 ? request.CorporateIds : null
        };

        int notificationId = await SaveNotificationAsync(notification);

        await SaveUserNotificationAsync(new SaveUserNotificationRequest
        {
            NotificationId = notificationId,
            Tokens = tokens.DistinctBy(x => new { x.UserId, x.Role }).ToList(),
        });
    }

    public async Task<PaginationResponse<AdminNotificationListingResponse>> NotificationListForAdminAsync(PaginationFilter request, CancellationToken cancellationToken)
    {
        var notificationQuery = _notificationRepository.GetAll();
        var response = await notificationQuery
            .Select(n => new AdminNotificationListingResponse
            {
                Id = n.Id,
                Title = n.Title,
                ImageUrl = n.Image!.AccessURL,
                Url = n.Url,
                Description = n.Description,
                CustomerIds = n.CustomerIds,
                DriverIds = n.DriverIds,
                CorporateIds = n.CorporateIds,
                CreatedOn = n.CreatedOn,
                LastModifiedOn = n.LastModifiedOn
            })
            .PaginatedListAsync(request);

        var distinctCustomerIds = response.Data.Where(n => n.CustomerIds != null && n.CustomerIds.Count > 0).SelectMany(n => n.CustomerIds!)?.Select(c => c)?.Distinct().ToList();
        var customers = _customerRepository.GetAll().Where(c => distinctCustomerIds!.Contains(c.Id)).Select(c => new LookupResponse(c.Id, c.Name!)).ToList();

        var distinctDriverIds = response.Data.Where(n => n.DriverIds != null && n.DriverIds.Count > 0).SelectMany(n => n.DriverIds!)?.Select(c => c)?.Distinct().ToList();
        var drivers = _driverRepository.GetAll().Where(c => distinctDriverIds!.Contains(c.Id)).Select(c => new LookupResponse(c.Id, c.Name!)).ToList();

        var distinctCorporateIds = response.Data.Where(n => n.CorporateIds != null && n.CorporateIds.Count > 0).SelectMany(n => n.CorporateIds!)?.Select(c => c)?.Distinct().ToList();
        var corporates = _corporateRepository.GetAll().Where(c => distinctCorporateIds!.Contains(c.Id)).Select(c => new LookupResponse(c.Id, c.Name!)).ToList();

        bool isGlobalAdmin = _currentUser.IsGlobalAdminUser();
        var currentUserPermissions = new PermissionDTO();

        if (!isGlobalAdmin)
            currentUserPermissions = await _adminRolePermissionService.GetModulePermissionsOfCurrentUserAsync(AdminModule.Notifications);

        foreach (var notification in response.Data)
        {
            if (notification.CustomerIds != null)
            {
                if (notification.CustomerIds.Count > 0)
                {
                    notification.Customers = customers.Where(c => notification.CustomerIds.Contains(c.Id)).ToList();
                }
                else
                {
                    notification.UserType?.Add(UserType.Users);
                }
            }

            if (notification.DriverIds != null)
            {
                if (notification.DriverIds.Count > 0)
                {
                    notification.Drivers = drivers.Where(d => notification.DriverIds.Contains(d.Id)).ToList();
                }
                else
                {
                    notification.UserType?.Add(UserType.Drivers);
                }
            }

            if (notification.CorporateIds != null)
            {
                if (notification.CorporateIds.Count > 0)
                {
                    notification.Corporates = corporates.Where(c => notification.CorporateIds.Contains(c.Id)).ToList();
                }
                else
                {
                    notification.UserType?.Add(UserType.Corporate);
                }
            }

            if (isGlobalAdmin || currentUserPermissions?.View == true)
            {
                notification.Actions.Add(ActionType.View.ToActionResponse());
            }
        }

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
}
