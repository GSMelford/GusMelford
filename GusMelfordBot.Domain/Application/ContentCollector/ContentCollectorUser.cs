namespace GusMelfordBot.Domain.Application.ContentCollector;

public class ContentCollectorUser
{
    public Guid Id { get; set; }
    public string ConnectionId { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; } = null!;
    public bool IsReady { get; set; }
}