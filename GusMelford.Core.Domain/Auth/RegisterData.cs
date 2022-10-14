namespace GusMelfordBot.Domain.Auth;

public class RegisterData
{
    public string FirstName { get; }
    public string? LastName { get; }
    public string Email { get; }
    public string Password { get; }

    public RegisterData(string firstName, string? lastName, string email, string password)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
    }
}