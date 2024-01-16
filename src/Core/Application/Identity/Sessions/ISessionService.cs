namespace Demo.WebApi.Application.Identity.Sessions;
public interface ISessionService : ITransientService
{
    Task CreateSessionAsync(string token, string UserId, string UserRole, string? DeviceId, DateTime ExpiryDate, string? DeviceName, string? FcmToken, bool LogoutExistingSessions = true);
    Task<bool> VerifyTokenSessionAsync(string token);
    Task LogOutSessionAsync(string token);
    Task LogOutAllSessionsAsync(string userId);
}