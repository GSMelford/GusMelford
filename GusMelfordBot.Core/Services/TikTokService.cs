namespace GusMelfordBot.Core.Services
{
    using Settings;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json.Linq;
    using Telegram.API.TelegramRequests.SendVideo;
    using Telegram.Dto.SendMessage.ReplyMarkup.Abstracts;
    using Telegram.Dto.SendMessage.ReplyMarkup.InlineKeyboard;
    using VideoFile = Telegram.API.TelegramRequests.SendVideo.VideoFile;
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
        private readonly CommonSettings _commonSettings;
        private string _currentVideoInfoMessageId;
        private int _videoCounter;

        public TikTokService(
            IDatabaseManager manager,
            IRequestService requestService,
            IGusMelfordBotService gusMelfordBotService,
            IPlayerService playerService,
            CommonSettings commonSettings)
        {
            _databaseManager = manager;
            _videoCounter = _databaseManager.Count<DAL.TikTok.Video>().Result;
            _users = _databaseManager.Get<DAL.User>();
            _gusMelfordBotService = gusMelfordBotService;
            _gusMelfordBotService.OnMessageUpdate += ProcessMessage;
            _gusMelfordBotService.OnCallbackQueryUpdate += ProcessCallbackQuery;
            _requestService = requestService;
            _playerService = playerService;
            _commonSettings = commonSettings;
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
                    Video = new VideoFile(await _playerService.GetCurrentVideoFileStream(), "video")
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

            HttpResponseMessage httpResponseMessage = await _requestService.ExecuteAsync(requestMessage);
            Uri uri = httpResponseMessage.RequestMessage?.RequestUri;
            
            requestMessage =
                new Request(uri?.ToString())
                    .AddHeaders(new Dictionary<string, string> {{"User-Agent", Constants.UserAgent}})
                    .Build();
            
            httpResponseMessage = await _requestService.ExecuteAsync(requestMessage);
            uri = httpResponseMessage.RequestMessage?.RequestUri;
            
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

        public async Task SendVideoInfo()
        {
            DAL.TikTok.Video video = _playerService.GetCurrentVideo();
            InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup();
            KeyboardRaw<InlineKeyboardButton> keyboardRaw = new KeyboardRaw<InlineKeyboardButton>();
            
            keyboardRaw.AddButtons(new List<InlineKeyboardButton>
            {
                new () {
                    Text = TikTokCallbackQueryButton.Like,
                    CallbackData = TikTokCallbackQueryButton.Like + " " + video.Id
                },
                new () {
                    Text = TikTokCallbackQueryButton.Save,
                    CallbackData = TikTokCallbackQueryButton.Save + " " + video.Id
                }
            });
            
            inlineKeyboardMarkup.AddRaw(keyboardRaw);
            HttpResponseMessage httpResponseMessage = await _gusMelfordBotService.SendMessage(
                new SendMessageParameters
                {
                    ChatId = _commonSettings.TikTokSettings.TikTokChatId,
                    Text = "Gus Melford Bot Playing now... 🥵\n\n" +
                           $"Video № {video.Id}\n" +
                           $"From user {video.User.FirstName}\n" +
                           $"{video.RefererLink}",
                    ReplyMarkup = inlineKeyboardMarkup
                });

            _currentVideoInfoMessageId =
                JToken.Parse(await httpResponseMessage.Content.ReadAsStringAsync())["result"]?["message_id"]
                    ?.ToString();
        }

        public async Task DeleteVideoInfo()
        {
            if (string.IsNullOrEmpty(_currentVideoInfoMessageId))
            {
                return;
            }
            
            await _gusMelfordBotService.DeleteMessage(
                new DeleteMessageParameters
                {
                    ChatId = _commonSettings.TikTokSettings.TikTokChatId,
                    MessageId = int.Parse(_currentVideoInfoMessageId)
                });
        }
    }
    
    public static class TikTokCallbackQueryButton
    {
        public const string Like = "❤️";
        public const string Save = "💾";
    }
}