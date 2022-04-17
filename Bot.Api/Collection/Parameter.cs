namespace Bot.Api.Collection
{
    public class Parameter
    {
        public string Key { get; }
        public object Value { get; }
        public UrlEncode UrlEncode { get; }
        
        public Parameter(string key, object value, UrlEncode urlEncode = UrlEncode.NoEncode)
        {
            Key = key;
            Value = value;
            UrlEncode = urlEncode;
        }
    }

    public enum UrlEncode
    {
        Encode,
        NoEncode
    }
}