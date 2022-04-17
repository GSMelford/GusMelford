namespace Telegram.Dto
{
    public class ChatId
    {
        private readonly long _identifier;

        private readonly string _username;

        private ChatId(string username)
        {
            _username = username;
        }
        
        private ChatId(long identifier)
        {
            _identifier = identifier;
        }
        
        public static implicit operator ChatId(long identifier)
        {
            return new(identifier);
        }
        
        public static implicit operator ChatId(string username)
        {
            return new(username);
        }

        public override string ToString()
        {
            return _username ?? _identifier.ToString();
        }
    }
}