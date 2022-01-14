using System.Threading.Tasks;
using GusMelfordBot.Core.Applications.MemesChatApp.Player.Entities;

namespace GusMelfordBot.Core.Applications.MemesChatApp.Interfaces
{
    public interface IPlayerService
    {
        byte[] CurrentContentBytes { get; set; }
        Task<PlayerInfo> SetNextVideo();
        Task<PlayerInfo> SetPreviousVideo();
    }
}