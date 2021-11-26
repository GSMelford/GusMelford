namespace GusMelfordBot.Core.Services.PlayerServices
{
    using System;
    
    public class VideoInfo
    {
        public string PlayerNameVersion { get; set; }
        public string Id { get; set; }
        public string VideoSenderName { get; set; }
        public string RefererLink { get; set; }
        public DateTime DateTime { get; set; }
    }
}