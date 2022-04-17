namespace Bot.Api.BotRequests
{
    using System;
    using Bot.Api.Collection;
    
    public class ParameterNameAttribute : Attribute
    {
        public string ParameterName { get; }

        public bool IsRequired { get; }

        public UrlEncode UrlEncode { get; }
        
        public ParameterNameAttribute(
            string parameterName, 
            bool isRequired = false, 
            UrlEncode urlEncode = UrlEncode.NoEncode)
        {
            ParameterName = parameterName;
            IsRequired = isRequired;
            UrlEncode = urlEncode;
        }
    }
}