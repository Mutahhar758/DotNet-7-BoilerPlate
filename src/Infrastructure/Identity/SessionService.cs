using Demo.WebApi.Application.Common.Exceptions;
using Demo.WebApi.Application.Common.Persistence;
using Demo.WebApi.Application.Identity.Sessions;
using Demo.WebApi.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Demo.WebApi.Infrastructure.Identity;
public class SessionService : ISessionService
{
    private readonly IRepository<UserSession> _userSessions;
    private readonly IStringLocalizer<SessionService> _localizer;
    public SessionService(IRepository<UserSession> userSessions, IStringLocalizer<SessionService> localizer)
    {
        _userSessions = userSessions;
        _localizer = localizer;
    }

    public async Task CreateSessionAsync(string token, string UserId, string UserRole, string? DeviceId, DateTime ExpiryDate, string? DeviceName, string? FcmToken, bool LogoutExistingSessions = true)
    {
        if (LogoutExistingSessions)
        {
            var existingSessions = await _userSessions.GetAll().Where(x => x.UserId == UserId && x.UserRole == UserRole).ToListAsync();
            existingSessions.ForEach(x => x.ExpiryDate = DateTime.UtcNow);
            await _userSessions.UpdateRangeAsync(existingSessions);
        }

        UserSession newSession = new UserSession
        {
            Token = token,
            UserId = UserId,
            DeviceId = DeviceId,
            DeviceName = DeviceName,
            FcmToken = FcmToken,
            ExpiryDate = ExpiryDate,
            UserRole = UserRole
        };
        await _userSessions.AddAsync(newSession);
    }

    public async Task<bool> VerifyTokenSessionAsync(string token)
    {
        DateTime now = DateTime.UtcNow;
        var session = await _userSessions.GetAll().FirstOrDefaultAsync(x => x.Token == token);

        if (session == null || session.ExpiryDate < now)
            return false;

        return true;
    }

    public async Task LogOutSessionAsync(string token)
    {
        token = token.Replace("Bearer", string.Empty).Trim();
        var session = await _userSessions.GetAll().FirstOrDefaultAsync(x => x.Token == token);

        if (session == null)
            throw new NotFoundException("Session not found");

        session.ExpiryDate = DateTime.UtcNow;
        await _userSessions.UpdateAsync(session);
    }

    public async Task LogOutAllSessionsAsync(string userId)
    {
        var sessions = await _userSessions.GetAll()
                                          .Where(x => x.UserId == userId && x.ExpiryDate > DateTime.UtcNow)
                                          .ToListAsync();

        foreach (var session in sessions) session.ExpiryDate = DateTime.UtcNow;
        await _userSessions.UpdateRangeAsync(sessions);
    }
}
