namespace GusMelfordBot.Core.Domain.System;

public interface ISystemRepository
{
    Task<bool> CheckCredentials(int telegramId, string? password);
    Task AddUser(UserDomain userDomain);
}