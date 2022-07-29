namespace GusMelfordBot.Domain.Application.ContentCollector;

public class UserDomain
{
    public string FirstName { get; }
    public string? LastName { get; }

    public UserDomain(string firstName, string? lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}