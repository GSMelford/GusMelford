namespace GusMelfordBot.Core.Interfaces
{
    using System.IO;
    using System;
    using DAL.TikTok;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    
    public interface IPlayerService
    {
        Task<FileStreamResult> GetNextVideoStream(Func<Video, bool> getPredicate);
        Stream GetCurrentVideoStream();
    }
}