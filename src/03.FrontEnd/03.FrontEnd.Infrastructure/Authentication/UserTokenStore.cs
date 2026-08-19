using EBVL.FrontEnd.Infrastructure.Authentication.Models;

namespace EBVL.FrontEnd.Infrastructure.Authentication;

public static class UserTokenStore
{
    private static readonly Dictionary<Guid, UserTokenSession> _sessions = [];
    private static readonly Lock _lock = new();

    public static Guid CreateSession(string userToken, string ipAddress, string userAgent)
    {
        lock (_lock)
        {
            var sessionId = Guid.NewGuid();

            _sessions[sessionId] = new UserTokenSession
            {
                UserToken = userToken,
                ExpiredAt = DateTimeOffset.Now.AddMinutes(1),
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            return sessionId;
        }
    }

    public static UserTokenSession? GetSession(Guid sessionId)
    {
        lock (_lock)
        {
            if (!_sessions.TryGetValue(sessionId, out var session))
            {
                return null;
            }

            if (session.ExpiredAt < DateTimeOffset.Now)
            {
                _ = _sessions.Remove(sessionId);

                return null;
            }

            return session;
        }
    }

    public static void RemoveSession(Guid sessionId)
    {
        lock (_lock)
        {
            _ = _sessions.Remove(sessionId);
        }
    }
}
