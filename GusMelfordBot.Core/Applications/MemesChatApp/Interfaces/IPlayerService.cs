using System.Threading.Tasks;
using GusMelfordBot.Core.Applications.MemesChatApp.Player.Entities;
using Telegram.Dto.UpdateModule;

namespace GusMelfordBot.Core.Applications.MemesChatApp.Interfaces;

public interface IPlayerService
{
    byte[] CurrentContentBytes { get; set; }
    Task<PlayerInfo> SetNextVideo();
    Task<PlayerInfo> SetPreviousVideo();
    Task ProcessCallbackQuery(CallbackQuery updateCallbackQuery);
    Task SetNotViewed();
    Task SetRandom(int number);
}