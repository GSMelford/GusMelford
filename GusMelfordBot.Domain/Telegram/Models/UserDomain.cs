namespace GusMelfordBot.Domain.Telegram.Models;

public class UserDomain
{
    public long? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }

    public UserDomain(long? id, string? firstName, string? lastName, string? username)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Username = username;
    }
}