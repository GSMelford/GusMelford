using GusMelfordBot.Domain.Application.ContentCollector;

namespace GusMelfordBot.Api.Services.Applications;

public class ContentCollectorSessionService
{
    private readonly Dictionary<Guid, List<SessionUser>> _sessions = new ();

    public Guid Create()
    {
        Guid sessionId = Guid.NewGuid();
        _sessions.Add(sessionId, new List<SessionUser>());
        return sessionId;
    }
    
    public void AddUser(Guid sessionId, SessionUser sessionUser)
    {
        _sessions[sessionId].Add(sessionUser);
    }
    
    public void RemoveUser(Guid sessionId, SessionUser sessionUser)
    {
        _sessions[sessionId].Remove(sessionUser);
    }
}