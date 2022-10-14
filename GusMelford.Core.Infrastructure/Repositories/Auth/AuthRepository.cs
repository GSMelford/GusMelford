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
    
    public async Task<Guid> RegisterUserIfNotExist(RegisterData registerData)
    {
        User? user = await _databaseContext.Set<User>()
            .FirstOrDefaultAsync(x => x.Email == registerData.Email);

        if (user is null)
        {
            user = new User
            {
                Email = registerData.Email,
                FirstName = registerData.FirstName,
                LastName = registerData.LastName,
                Password = registerData.Password,
                Role = await _databaseContext.Set<Role>().FirstOrDefaultAsync(x => x.Name == "User")
            };

            await _databaseContext.AddAsync(user);
            await _databaseContext.SaveChangesAsync();
        }
        
        return user.Id;
    }
    
    public async Task<Guid> RegisterUserFromTelegramIfNotExist(RegisterData registerData, string? username, long telegramId)
    {
        TelegramUser? telegramUser = await _databaseContext.Set<TelegramUser>()
            .FirstOrDefaultAsync(x => x.TelegramId == telegramId);
        
        registerData = SetTempEmail(registerData, $"{username}.{telegramId}@gmail.com");
        Guid userId = await RegisterUserIfNotExist(registerData);
        User user = await _databaseContext.Set<User>().FirstAsync(x => x.Id == userId);
        
        if (telegramUser is null)
        {
            telegramUser = new TelegramUser
            {
                User = user,
                Username = username,
                TelegramId = telegramId
            };

            await _databaseContext.AddAsync(telegramUser);
            await _databaseContext.SaveChangesAsync();
        }

        return userId;
    }

    private RegisterData SetTempEmail(RegisterData registerData, string tempEmail)
    {
        return new RegisterData(registerData.FirstName, registerData.LastName, tempEmail, registerData.Password);
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