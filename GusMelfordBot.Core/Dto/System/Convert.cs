using GusMelfordBot.Core.Domain.System;

namespace GusMelfordBot.Core.Dto.System;

public static class Convert
{
    public static SystemInfoDto ToDto(this SystemInfo systemInfo)
    {
        return new SystemInfoDto
        {
            SystemName = systemInfo.SystemName,
            SystemVersion = systemInfo.SystemVersion
        };
    }
}