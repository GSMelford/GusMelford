namespace GusMelfordBot.Domain.Auth;

public interface IAuthRepository
{
    Task<AuthUserDomain?> AuthenticateUser(long telegramId, string password);
    Task<AuthUserDomain> AuthenticateUser(Guid userId);
    Task UpdatePasswordAsync(long telegramId, string password);
    Task UpdateRefreshTokenAsync(Guid userId, string refreshToken);
}