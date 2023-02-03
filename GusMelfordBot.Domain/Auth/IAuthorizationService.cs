namespace GusMelfordBot.Domain.Auth;

public interface IAuthorizationService
{
    Task<Jwt> LoginAsync(TelegramLoginData telegramLoginData);
    Task<Jwt> RefreshTokenAsync(TokensDomain tokensDomain);
}