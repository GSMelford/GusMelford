using GusMelfordBot.Core.Domain.System;

namespace GusMelfordBot.Core.Services.System;

public class SystemService : ISystemService
{
    public SystemInfo BuildSystemInfo(string systemName, string systemVersion)
    {
        return new SystemInfo
        {
            SystemName = systemName,
            SystemVersion = systemVersion
        };
    }
}