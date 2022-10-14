namespace GusMelford.TelegramBot.Domain.Telegram;

public class ChatDomain
{
    public long Id { get; }
    public string Type { get; }
    public string? Title { get; }
    public string? Username { get; }
    public string? FirstName { get; }
    public string? LastName { get; }
    public bool? AllMembersAreAdministrators { get; }

    public ChatDomain(
        long id, 
        string type, 
        string? title, 
        string? username,
        string? firstName, 
        string? lastName, 
        bool? allMembersAreAdministrators)
    {
        Id = id;
        Type = type;
        Title = title;
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        AllMembersAreAdministrators = allMembersAreAdministrators;
    }
}