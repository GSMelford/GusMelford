namespace GusMelfordBot.Domain.Auth;

public class Jwt
{
    public string UserFullName { get; }
    public string Role { get; }
    public string AccessToken { get; }
    public DateTime ExpiredIn { get; }
    public string RefreshToken { get; }

    public Jwt(string userFullName, string role, string accessToken, DateTime expiredIn, string refreshToken)
    {
        UserFullName = userFullName;
        Role = role;
        AccessToken = accessToken;
        ExpiredIn = expiredIn;
        RefreshToken = refreshToken;
    }
}