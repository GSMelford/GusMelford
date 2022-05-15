using Bot.Api.BotRequests;

namespace Telegram.API.TelegramRequests.GetFile
{
    public class GetFileParameters : SerializeParameters
    {
        [ParameterName("file_id", true)]
        public string FileId { get; set; }
    }
}