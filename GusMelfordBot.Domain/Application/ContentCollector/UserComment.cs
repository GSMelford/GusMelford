namespace GusMelfordBot.Domain.Application.ContentCollector;

public class UserComment
{
    public Guid UserId { get; }
    public string Message { get; }

    public UserComment(Guid userId, string message)
    {
        UserId = userId;
        Message = message;
    }
}