namespace GusMelfordBot.Domain.Auth;

public class TokensDomain
{
    public string AccessToken { get; }
    public string RefreshToken { get; }

    public TokensDomain(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}