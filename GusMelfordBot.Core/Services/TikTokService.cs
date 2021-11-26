using System.IO;
using GusMelfordBot.Core.Settings;
using Microsoft.EntityFrameworkCore;
using Telegram.API.TelegramRequests.SendVideo;

namespace GusMelfordBot.Core.Services
{
    using Requests;
    using System.Threading.Tasks;
    using Telegram.API.TelegramRequests.DeleteMessage;
    using Telegram.API.TelegramRequests.SendMessage;
    using Telegram.Dto;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using GusMelfordBot.Database.Interfaces;
    using Interfaces;
    using System.Linq;
    using Telegram.Dto.UpdateModule;
    
    public class TikTokService : ITikTokService
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly List<DAL.User> _users;
        private readonly IGusMelfordBotService _gusMelfordBotService;
        private readonly IRequestService _requestService;
        private readonly IPlayerService _playerService;
        private int _videoCounter;

        public TikTokService(
            IDatabaseManager manager,
            IRequestService requestService,
            IGusMelfordBotService gusMelfordBotService,
            IPlayerService playerService)
        {
            _databaseManager = manager;
            _videoCounter = _databaseManager.Count<DAL.TikTok.Video>().Result;
            _users = _databaseManager.Get<DAL.User>();
            _gusMelfordBotService = gusMelfordBotService;
            _gusMelfordBotService.OnMessageUpdate += ProcessMessage;
            _gusMelfordBotService.OnCallbackQueryUpdate += ProcessCallbackQuery;
            _requestService = requestService;
            _playerService = playerService;
        }

        private async void ProcessCallbackQuery(CallbackQuery callbackQuery)
        {
            string[] data = callbackQuery.Data.Split(" ");

            switch (data[0])
            {
                case TikTokCallbackQueryButton.Save:
                {
                    await SendVideo(callbackQuery, data);
                    break;
                }
            }
        }
        
        private async Task SendVideo(CallbackQuery callbackQuery, string[] data)
        {
            DAL.TikTok.Video video = await _databaseManager.Context.Set<DAL.TikTok.Video>()
                .FirstOrDefaultAsync(v=>string.Equals(v.Id.ToString(), data[1]));
                    
            if (video != null)
            {
                await _gusMelfordBotService.SendVideoAsync(new SendVideoParameters
                {
                    ChatId = callbackQuery.FromUser.Id,
                    Video = new VideoFile(_playerService.GetCurrentVideoStream(), "video")
                });
            }
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
            
            await _databaseManager.SaveAllAcync();
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
            
            HttpRequestMessage requestMessage =
                new Request(videoLink)
                    .AddHeaders(new Dictionary<string, string> {{"User-Agent", Constants.UserAgent}})
                    .Build();
            
            Uri uri = (await _requestService.ExecuteAsync(requestMessage)).RequestMessage?.RequestUri;
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
            await _databaseManager.SaveAllAcync();

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
        
        private static bool VerifyTikTokMessage(Message message)
        {
            return Constants.TikTokDomains.FirstOrDefault(x => message.Text.Contains(x)) is not null;
        }
    }
}