using GusMelfordBot.Domain.Auth;
using GusMelfordBot.Infrastructure.Models;

namespace GusMelfordBot.Infrastructure.Repositories.Auth;

public static class Convertor
{
    public static AuthUserDomain ToDomain(this TelegramUser telegramUser)
    {
        return new AuthUserDomain(
            telegramUser.User.Id, 
            telegramUser.User.Role!.Name, 
            telegramUser.User.FirstName, 
            telegramUser.User.LastName,
            telegramUser.User.RefreshToken);
    }

    public static AuthUserDomain ToDomain(this User user)
    {
        return new AuthUserDomain(
            user.Id, 
            user.Role!.Name, 
            user.FirstName,
            user.LastName,
            user.RefreshToken);
    }
}