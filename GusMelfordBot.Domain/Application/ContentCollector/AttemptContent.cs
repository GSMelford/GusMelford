namespace GusMelfordBot.Domain.Application.ContentCollector;

public class AttemptContent
{
    public Guid SessionId { get; }
    public Guid GroupId { get; }
    public Guid UserId { get; }
    public string Message { get; }
    public int Attempt { get; }

    public AttemptContent(Guid sessionId, Guid groupId, Guid userId, string message, int attempt)
    {
        SessionId = sessionId;
        GroupId = groupId;
        UserId = userId;
        Message = message;
        Attempt = attempt;
    }
}