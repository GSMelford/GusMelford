namespace GusMelfordBot.Domain.Application.ContentCollector;

public class ContentUserDomain
{
    public string FirstName { get; }
    public string? LastName { get; }

    public ContentUserDomain(string firstName, string? lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}