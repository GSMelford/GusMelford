namespace GusMelfordBot.Domain.Auth;

public interface IAuthService
{
    Task<Jwt> LoginAsync(TelegramLoginData telegramLoginData);
    Task<Jwt> RefreshTokenAsync(TokensDomain tokensDomain);
}