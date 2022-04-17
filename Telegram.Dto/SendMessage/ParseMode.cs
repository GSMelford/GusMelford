namespace Telegram.Dto.SendMessage
{
    public class ParseMode
    {
        private ParseMode(string parseMode) { ParseModeValue = parseMode; }

        private string ParseModeValue { get; }

        public static ParseMode Markdown => new ("Markdown");
        public static ParseMode Html => new ("Html");

        public override string ToString()
        {
            return ParseModeValue;
        }
    }
}