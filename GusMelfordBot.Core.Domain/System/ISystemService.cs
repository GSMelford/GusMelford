namespace GusMelfordBot.Core.Domain.System;

public interface ISystemService
{
    SystemInfo BuildSystemInfo(string? systemName, string? systemVersion);
}