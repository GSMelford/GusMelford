namespace GusMelfordBot.Core.Applications.MemesChatApp.ContentProviders.TikTok
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Services.Requests;
    using GusMelfordBot.DAL.Applications.MemesChat.TikTok;
    using GusMelfordBot.Database.Interfaces;
    using GusMelfordBot.Core.Interfaces;
    
    public class TikTokServiceHelper
    {
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

        public string WithdrawRefererLink(IRequestService requestService, string sentLink)
        {
            HttpRequestMessage requestMessage =
                new Request(sentLink)
                    .AddHeaders(new Dictionary<string, string> {{"User-Agent", Constants.UserAgent}})
                    .Build();

            HttpResponseMessage httpResponseMessage = requestService.ExecuteAsync(requestMessage).Result;
            Uri uri = httpResponseMessage.RequestMessage?.RequestUri;

            requestMessage =
                new Request(uri?.ToString())
                    .AddHeaders(new Dictionary<string, string> {{"User-Agent", Constants.UserAgent}})
                    .Build();

            httpResponseMessage = requestService.ExecuteAsync(requestMessage).Result;
            uri = httpResponseMessage.RequestMessage?.RequestUri;

            string refererLink = string.Empty;
            if (uri is not null)
            {
                refererLink = uri.Scheme + "://" + uri.Host + uri.AbsolutePath;
            }

            return refererLink;
        }
    }
}