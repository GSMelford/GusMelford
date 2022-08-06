namespace GusMelfordBot.Domain.Auth;

public class Jwt
{
    public string UserFullName { get; }
    public string AccessToken { get; }

    public Jwt(string userFullName, string accessToken)
    {
        UserFullName = userFullName;
        AccessToken = accessToken;
    }
}