namespace GusMelfordBot.Core.Services
{
    using System.Threading.Tasks;
    using Telegram.API.TelegramRequests.DeleteMessage;
    using Telegram.API.TelegramRequests.SendMessage;
    using Telegram.Dto;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using DAL.TikTok;
    using GusMelfordBot.Database.Interfaces;
    using Interfaces;
    using System.Linq;
    using Telegram.Dto.UpdateModule;
    
    public class TikTokService : ITikTokService
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly List<DAL.User> _users;
        private readonly HttpClient _httpClient;
        private readonly IGusMelfordBotService _gusMelfordBotService;
        private int _videoCounter;
        
        public TikTokService(
            IDatabaseManager manager, 
            IGusMelfordBotService gusMelfordBotService)
        {
            _httpClient = new HttpClient();
            _databaseManager = manager;
            _videoCounter = _databaseManager.Count<DAL.TikTok.Video>().Result;
            _users = _databaseManager.Get<DAL.User>();
            _gusMelfordBotService = gusMelfordBotService;
            _gusMelfordBotService.OnMessageUpdate += ProcessMessage;
        }

        private async void ProcessMessage(Message message)
        {
            if (!VerifyTikTokMessage(message))
            {
                return;
            }

            await AddUserIfNotExist(message.From);
            SaveLink(message);
        }

        private async Task AddUserIfNotExist(User user)
        {
            if (_users.FirstOrDefault(x => x.TelegramUserId == user.Id) is not null)
            {
                return;
            }
            
            _users.Add(new DAL.User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.Username,
                TelegramUserId = user.Id
            });
            
            await _databaseManager.SaveAll();
        }

        private async void SaveLink(Message message)
        {
            string text = message.Text;
            string videoLink = text.Split(' ').FirstOrDefault(l =>
                l.Contains(Constants.TikTokDomain) || l.Contains(Constants.TikTokMobileDomain))?.Trim();

            if (string.IsNullOrEmpty(videoLink))
            {
                return;
            }
            
            string signature = text.Replace(videoLink, "");
            Uri uri = DoRequest(videoLink).RequestMessage?.RequestUri;
            string referer = string.Empty;
            
            if (uri is not null)
            {
                referer = uri.Scheme + "://" + uri.Host + uri.AbsolutePath;
            }

            var user = _users.FirstOrDefault(u => u.TelegramUserId == message.From.Id);
            
            var video = new DAL.TikTok.Video {
                User = user,
                SentLink = videoLink,
                RefererLink = referer,
                Signature = signature
            };
            
            await _databaseManager.Add(video);
            await _databaseManager.SaveAll();

            SendMessage(message.Chat, $"{user?.FirstName} sent {++_videoCounter}\n{videoLink}");
            DeleteMessage(message);
        }

        private void DeleteMessage(Message message)
        {
            _gusMelfordBotService.DeleteMessage(
                new DeleteMessageParameters
                {
                    ChatId = message.Chat.Id,
                    MessageId = message.MessageId
                });
        }

        private void SendMessage(Chat chat, string message)
        {
            _gusMelfordBotService.SendMessage(
                new SendMessageParameters
                {
                    Text = message,
                    ChatId = chat.Id,
                    DisableNotification = true,
                    DisableWebPagePreview = true
                });
        }

        private HttpResponseMessage DoRequest(string requestUrl)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            httpRequestMessage.Headers.Add("User-Agent", Constants.UserAgent);
            return _httpClient.SendAsync(httpRequestMessage).Result;
        }
        
        private static bool VerifyTikTokMessage(Message message)
        {
            return Constants.TikTokDomains.FirstOrDefault(x => message.Text.Contains(x)) is not null;
        }
    }
}