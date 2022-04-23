using GusMelfordBot.Core.Domain.System;
using GusMelfordBot.Core.Extensions;

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

    public static Credentials ToDomain(this CredentialsDto credentialsDto)
    {
        return new Credentials
        {
            Password = credentialsDto.Password,
            TelegramId = credentialsDto.TelegramId.ToInt()
        };
    }
    
    public static UserDomain ToDomain(this UserDto userDto)
    {
        return new UserDomain
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            UserName = userDto.UserName,
            TelegramUserId = userDto.TelegramUserId.ToInt()
        };
    }
}