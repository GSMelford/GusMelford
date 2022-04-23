namespace GusMelfordBot.Core.Domain.System;

public interface ISystemService
{
    SystemInfo BuildSystemInfo(string? systemName, string? systemVersion);
    Task<bool> Login(Credentials credentials);
    Task AddUser(UserDomain userDomain);
}