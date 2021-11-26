namespace GusMelfordBot.Core.Interfaces
{
    using System.Threading.Tasks;
    
    public interface ITikTokService
    {
        Task SendVideoInfo();
        Task DeleteVideoInfo();
    }
}