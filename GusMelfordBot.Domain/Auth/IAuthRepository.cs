namespace GusMelfordBot.Domain.Auth;

public interface IAuthRepository
{
    Task<AuthUserDomain?> AuthenticateUser(long telegramId, string password);
    Task<AuthUserDomain> AuthenticateUser(Guid userId);
    Task UpdatePasswordAsync(long telegramId, string password);
    Task UpdateRefreshTokenAsync(Guid userId, string refreshToken);
    Task SaveUserAsync(RegisterData registerData);
    Task<bool> IsTelegramUserExistAsync(long telegramUserId);
    Task SaveTelegramUserAsync(RegisterData registerData, long telegramUserId, string userName);
}