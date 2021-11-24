namespace GusMelfordBot.Core.Services.Data
{
    using System.Collections.Generic;
    using DAL.TikTok;
    
    public class VideoData
    {
        public int Count { get; set; }
        public List<Video> Videos { get; set; }

        public VideoData(List<Video> videos)
        {
            Count = videos.Count;
            Videos = videos;
        }
    }
}