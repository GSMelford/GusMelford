using System.IO;

namespace Telegram.API.TelegramRequests.SendVideo
{
    public class VideoFile
    {
        public Stream VideoStream { get; set; }
        public string VideoName { get; set; }

        public VideoFile(Stream videoStream, string videoName)
        {
            VideoStream = videoStream;
            VideoName = videoName;
        }
    }
}