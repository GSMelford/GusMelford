namespace GusMelfordBot.Domain.Auth;

public interface IAuthRepository
{
    Task<Guid> RegisterUserIfNotExist(RegisterData registerData);
    Task<Guid> RegisterUserFromTelegramIfNotExist(RegisterData registerData, string? username, long telegramId);
}