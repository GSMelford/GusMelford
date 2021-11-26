namespace GusMelfordBot.Core.Services.PlayerServices
{
    using System.IO;
    
    public class VideoFile
    {
        public bool IsDownloaded { get; set; }
        public Stream Stream { get; set; }
        public byte[] VideoArray { get; set; }
    }
}