namespace GusMelfordBot.Core.Services.System;

using Data.Entities;
    
public class SystemInfo
{
    public string Name { get; set; }
    public string Version { get; set; }
    public PlayerInformation PlayerInformation { get; set; }
}