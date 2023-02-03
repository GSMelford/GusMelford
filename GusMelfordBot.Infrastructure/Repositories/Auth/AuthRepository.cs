using GusMelfordBot.Domain.Auth;
using GusMelfordBot.Extensions;
using GusMelfordBot.Extensions.Exceptions;
using GusMelfordBot.Infrastructure.Interfaces;
using GusMelfordBot.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace GusMelfordBot.Infrastructure.Repositories.Auth;

public class AuthRepository : IAuthRepository
{
    private readonly IDatabaseContext _databaseContext;
    
    public AuthRepository(IDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    public async Task<AuthUserDomain?> AuthenticateUser(long telegramId, string password)
    {
        return (await _databaseContext.Set<TelegramUser>()
            .Include(x => x.User)
            .ThenInclude(x=>x.Role)
            .FirstOrDefaultAsync(x => x.TelegramId == telegramId && x.User.Password == password))?
            .ToDomain();
    }
    
    public async Task<AuthUserDomain> AuthenticateUser(Guid userId)
    {
        return (await _databaseContext.Set<User>()
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == userId))
            !.ToDomain();
    }

    public async Task SaveUserAsync(RegisterData registerData)
    {
        User user = await BuildUserAsync(registerData);

        await _databaseContext.AddAsync(user);
        await _databaseContext.SaveChangesAsync();
    }
    
    public async Task SaveTelegramUserAsync(RegisterData registerData, long telegramUserId, string userName)
    {
        User user = await BuildUserAsync(registerData);
        TelegramUser telegramUser = new TelegramUser
        {
            User = user,
            TelegramId = telegramUserId,
            UserName = userName
        };

        await _databaseContext.AddAsync(telegramUser);
        await _databaseContext.SaveChangesAsync();
    }

    public async Task<bool> IsTelegramUserExistAsync(long telegramUserId)
    {
        return await _databaseContext.Set<TelegramUser>()
            .FirstOrDefaultAsync(x=> x.TelegramId == telegramUserId) is not null;
    }

    private async Task<User> BuildUserAsync(RegisterData registerData)
    {
        return new User
        {
            Email = registerData.Email,
            FirstName = registerData.FirstName,
            LastName = registerData.LastName,
            Role = (await _databaseContext.Set<Role>().FirstOrDefaultAsync(x => x.Name == "User"))!,
            Password = registerData.Password
        };
    }
    
    public async Task UpdatePasswordAsync(long telegramId, string password)
    {
        TelegramUser? telegramUser = await _databaseContext.Set<TelegramUser>()
            .Include(x=>x.User)
            .FirstOrDefaultAsync(x => x.TelegramId == telegramId);

        if (telegramUser is null)
        {
            return;
        }

        telegramUser.User.Password = password.ToSha512();
        _databaseContext.Update(telegramUser);
        await _databaseContext.SaveChangesAsync();
    }

    public async Task UpdateRefreshTokenAsync(Guid userId, string refreshToken)
    {
        User user = ( await _databaseContext.Set<User>().FirstOrDefaultAsync(x => x.Id == userId))
            .IfNullThrow(new UnauthorizedException($"User not found {userId}"));

        user.RefreshToken = refreshToken;
        _databaseContext.Update(user);
        await _databaseContext.SaveChangesAsync();
    }
}