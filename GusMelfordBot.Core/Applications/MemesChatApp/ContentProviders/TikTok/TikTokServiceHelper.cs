namespace GusMelfordBot.Core.Applications.MemesChatApp.ContentProviders.TikTok
{
    using System.Threading;
    using RestSharp;
    using System;
    using System.Linq;
    using GusMelfordBot.DAL.Applications.MemesChat.TikTok;
    using GusMelfordBot.Database.Interfaces;
    
    public class TikTokServiceHelper
    {
        private readonly RestClient _client = new ();
        
        public string WithdrawSendLink(string messageText)
        {
            return messageText.Split(' ', '\n')
                .FirstOrDefault(x => x.Contains(Constants.TikTok))?.Trim();
        }

        public TikTokVideoContent BuildTikTokVideoContent(
            IDatabaseManager databaseManager,
            string sentLink,
            string refererLink,
            long userId)
        {
            var user = databaseManager.Context.Set<DAL.User>()
                .FirstOrDefault(u => u.TelegramUserId == userId);

            return new TikTokVideoContent
            {
                User = user,
                SentLink = sentLink,
                RefererLink = refererLink,
                Description = null,
                AccompanyingCommentary = null
            };
        }

        public string WithdrawRefererLink(string sentLink)
        {
            RestResponse response = _client.ExecuteAsync(new RestRequest(sentLink)).Result;
            
            Uri uri = response.ResponseUri;
            response = _client.ExecuteAsync(new RestRequest(uri?.ToString())).Result;
            
            uri = response.ResponseUri;
            string refererLink = string.Empty;
            if (uri is not null)
            {
                refererLink = uri.Scheme + "://" + uri.Host + uri.AbsolutePath;
            }

            Thread.Sleep(1000);
            
            return refererLink;
        }
    }
}