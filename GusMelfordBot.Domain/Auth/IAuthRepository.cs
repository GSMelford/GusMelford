namespace GusMelfordBot.Domain.Auth;

public interface IAuthRepository
{
    Task<Guid> RegisterUserIfNotExist(RegisterData registerData);
    Task<Guid> RegisterUserFromTelegramIfNotExist(RegisterData registerData, string? username, long telegramId);
    Task<AuthUserDomain?> AuthenticateUser(long telegramId, string password);
    Task<AuthUserDomain> AuthenticateUser(Guid userId);
    Task UpdatePasswordAsync(long telegramId, string password);
    Task UpdateRefreshTokenAsync(Guid userId, string refreshToken);
}