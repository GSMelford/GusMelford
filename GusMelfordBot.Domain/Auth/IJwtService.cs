namespace GusMelfordBot.Domain.Auth;

public interface IJwtService
{
    Jwt BuildJwtToken(AuthUserDomain authUserDomain);
    bool IsValidToken(string accessToken, out Guid userId);
}