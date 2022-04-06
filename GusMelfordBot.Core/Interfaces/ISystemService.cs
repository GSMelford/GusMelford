using System.Threading.Tasks;
using GusMelfordBot.Core.Services.System;

namespace GusMelfordBot.Core.Interfaces;

using Services.Data.Entities;
    
public interface ISystemService
{
    Task<SystemInfo> GetSystemData();
}