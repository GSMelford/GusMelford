namespace GusMelfordBot.Domain.Auth;

public class AuthUserDomain
{
    public Guid Id { get; }
    public string Role { get; }
    public string FirstName { get; }
    public string? LastName { get; }
    public string? RefreshToken { get; }
    
    public AuthUserDomain(Guid id, string role, string firstName, string? lastName, string? refreshToken)
    {
        Id = id;
        Role = role;
        FirstName = firstName;
        LastName = lastName;
        RefreshToken = refreshToken;
    }
}