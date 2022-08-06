namespace GusMelfordBot.Domain.Auth;

public interface IAuthService
{
    Task<Jwt> Login(TelegramLoginData telegramLoginData);
}