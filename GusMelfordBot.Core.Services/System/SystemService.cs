using GusMelfordBot.Core.Domain.System;

namespace GusMelfordBot.Core.Services.System;

public class SystemService : ISystemService
{
    private readonly ISystemRepository _systemRepository;
    
    public SystemService(ISystemRepository systemRepository)
    {
        _systemRepository = systemRepository;
    }
    
    public SystemInfo BuildSystemInfo(string? systemName, string? systemVersion)
    {
        return new SystemInfo
        {
            SystemName = systemName,
            SystemVersion = systemVersion
        };
    }

    public async Task<bool> Login(Credentials credentials)
    {
        return await _systemRepository.CheckCredentials(credentials.TelegramId, credentials.Password);
    }
    
    public async Task AddUser(UserDomain userDomain)
    {
        await _systemRepository.AddUser(userDomain);
    }
}