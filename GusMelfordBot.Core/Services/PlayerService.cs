using GusMelfordBot.Core.Settings;
using Microsoft.EntityFrameworkCore;
using Telegram.API.TelegramRequests.DeleteMessage;
using Telegram.API.TelegramRequests.SendMessage;
using Telegram.Dto.SendMessage.ReplyMarkup.Abstracts;
using Telegram.Dto.SendMessage.ReplyMarkup.InlineKeyboard;

namespace GusMelfordBot.Core.Services
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Requests;
    using Microsoft.AspNetCore.Mvc;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using DAL.TikTok;
    using GusMelfordBot.Database.Interfaces;
    using Interfaces;
    
    public class PlayerService : IPlayerService
    {
        private readonly IRequestService _requestService;
        private readonly IDatabaseManager _databaseManager;
        private readonly ILogger<PlayerService> _logger;
        private readonly IGusMelfordBotService _gusMelfordBotService;
        private readonly CommonSettings _commonSettings;

        private const string ContentType = "application/octet-stream";

        private readonly List<Video> _videos;
        private Video _currentVideo;
        private Stream _currentVideoStream;
        private byte[] _currentVideoArray;
        private string _currentVideoInfoMessageId;

        public PlayerService(
            IDatabaseManager databaseManager, 
            IRequestService requestService,
            ILogger<PlayerService> logger,
            IGusMelfordBotService gusMelfordBotService,
            CommonSettings commonSettings)
        {
            _logger = logger;
            _requestService = requestService;
            _databaseManager = databaseManager;
            
            _videos = _databaseManager.Context
                .Set<Video>()
                .Include(video => video.User)
                .ToListAsync().Result;
            
            _gusMelfordBotService = gusMelfordBotService;
            _commonSettings = commonSettings;
        }

        public async Task<FileStreamResult> GetNextVideoStream(Func<Video, bool> getPredicate)
        {
            if (_currentVideo is not null)
            {
                _currentVideo.IsViewed = true;
                await DeleteVideoInfoFromTelegram();
            }
            
            do
            {
                _currentVideo = GetVideoOrRandom(getPredicate);
                _currentVideoStream = await GetVideoStream(GetOriginalLink(await GetVideoInformation()));

                if (_currentVideoStream is null)
                {
                    _currentVideo.IsValid = false;
                    continue;
                }
                
                _currentVideo.IsValid = true;
                
                await SendVideoInfoToTelegram();
                await _databaseManager.SaveAllAcync();
            } while (!_currentVideo.IsValid.Value);

            return new FileStreamResult(_currentVideoStream, ContentType);
        }

        public Stream GetCurrentVideoStream()
        {
            string bufferName = "temp.mp4";

            if (File.Exists(bufferName))
            {
                File.Delete(bufferName);
            }
            
            using (FileStream fileStream = new FileStream(bufferName, FileMode.CreateNew))
            {
                fileStream.Write(_currentVideoArray, 0, _currentVideoArray.Length);
            }
            
            return File.OpenRead(bufferName);
        }

        private async Task<Stream> GetVideoStream(string videoRequestUrl)
        {
            try
            {
                HttpRequestMessage requestMessage = new Request(videoRequestUrl)
                    .AddHeaders(new Dictionary<string, string>
                    {
                        {"User-Agent", Constants.UserAgent},
                        {"Referer", _currentVideo.RefererLink},
                    }).Build();

                HttpResponseMessage httpResponseMessage = await _requestService.ExecuteAsync(requestMessage);
                _currentVideoArray = await httpResponseMessage.Content.ReadAsByteArrayAsync();
                return await httpResponseMessage.Content.ReadAsStreamAsync();
            }
            catch
            {
                _logger.LogError("Video error {SentLink}", _currentVideo.SentLink);
                return null;
            }
        }
        
        private async Task<JToken> GetVideoInformation()
        {
            HttpRequestMessage requestMessage =
                new Request(BuildVideoInformationUrl(_currentVideo))
                    .AddHeaders(new Dictionary<string, string> {{"User-Agent", Constants.UserAgent}})
                    .Build();

            return await (await _requestService.ExecuteAsync(requestMessage)).GetJTokenAsync();
        }
        
        private Video GetVideoOrRandom(Func<Video, bool> getPredicate)
        {
            
            return _videos.FirstOrDefault(x => getPredicate(x) && x.IsValid.HasValue && x.IsValid.Value)
                   ?? _videos[new Random().Next(0, _videos.Count)];;
        }

        private string GetOriginalLink(JToken videoInformation)
        {
            return videoInformation["itemInfo"]?["itemStruct"]?["video"]?["downloadAddr"]?.ToString();
        }
        
        private string BuildVideoInformationUrl(Video video)
        {
            return $"https://www.tiktok.com/node/share/video/{GetVideoUser(video.RefererLink)}" +
                   $"/{GetVideoId(video.RefererLink)}";
        }
        
        private string GetVideoUser(string referer)
        {
            return Regex
                .Match(referer, "com/(.*?)/video")
                .Groups[1]
                .Value;
        }

        private string GetVideoId(string referer)
        {
            return referer
                .Replace(Constants.TikTokDomain, "")
                .Replace("/video/", " ")
                .Split(" ")[1];
        }

        private async Task SendVideoInfoToTelegram()
        {
            InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup();
            KeyboardRaw<InlineKeyboardButton> keyboardRaw = new KeyboardRaw<InlineKeyboardButton>();
            
            keyboardRaw.AddButtons(new List<InlineKeyboardButton>
            {
                new () {
                    Text = TikTokCallbackQueryButton.Like,
                    CallbackData = TikTokCallbackQueryButton.Like + " " + _currentVideo.Id
                },
                new () {
                    Text = TikTokCallbackQueryButton.Save,
                    CallbackData = TikTokCallbackQueryButton.Save + " " + _currentVideo.Id
                }
            });
            
            inlineKeyboardMarkup.AddRaw(keyboardRaw);
            HttpResponseMessage httpResponseMessage = await _gusMelfordBotService.SendMessage(
                new SendMessageParameters
                {
                    ChatId = _commonSettings.TikTokSettings.TikTokChatId,
                    Text = "Gus Melford Bot Playing now... 🥵\n\n" +
                           $"Video № {_currentVideo.Id}\n" +
                           $"From user {_currentVideo.User.FirstName}\n" +
                           $"{_currentVideo.RefererLink}",
                    ReplyMarkup = inlineKeyboardMarkup
                });

            _currentVideoInfoMessageId =
                JToken.Parse(await httpResponseMessage.Content.ReadAsStringAsync())["result"]?["message_id"]
                    ?.ToString();
        }

        private async Task DeleteVideoInfoFromTelegram()
        {
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