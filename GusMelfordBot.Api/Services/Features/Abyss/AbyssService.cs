using GusMelfordBot.Domain;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Events;
using GusMelfordBot.Extensions;
using GusMelfordBot.Extensions.Services.DataLake;
using GusMelfordBot.SimpleKafka.Interfaces;
using Newtonsoft.Json.Linq;
using TBot.Client;
using TBot.Client.Api.Telegram.DeleteMessage;
using TBot.Client.Api.Telegram.SendMessage;
using TBot.Client.Api.Telegram.SendVideo;

namespace GusMelfordBot.Api.Services.Features.Abyss;

public class AbyssService : IAbyssService
{
    private readonly ILogger<AbyssService> _logger;
    private readonly IAbyssRepository _abyssRepository;
    private readonly IKafkaProducer<string> _kafkaProducer;
    private readonly ITBot _tBot;
    private readonly IDataLakeService _dataLakeService;

    public AbyssService(
        ILogger<AbyssService> logger, 
        IAbyssRepository abyssRepository, 
        IKafkaProducer<string> kafkaProducer, 
        ITBot tBot, 
        IDataLakeService dataLakeService)
    {
        _logger = logger;
        _abyssRepository = abyssRepository;
        _kafkaProducer = kafkaProducer;
        _tBot = tBot;
        _dataLakeService = dataLakeService;
    }

    public async Task HandleAsync(AbyssContext abyssContext)
    {
        Guid? userId = await _abyssRepository.GetUserIdAsync(abyssContext.TelegramUserId);
        if (userId is null) {
            await _tBot.SendMessageAsync(new SendMessageParameters
            {
                ChatId = abyssContext.TelegramChatId,
                ReplyToMessageId = abyssContext.TelegramMessageId,
                Text = "Hi! In order to take part in throwing content into the abyss, you need to register! 😊"
            });
            return;
        }
        
        await _kafkaProducer.ProduceAsync(new ContentEvent
        {
            SessionId = abyssContext.SessionId,
            Message = abyssContext.Message,
            UserId = userId.Value,
            GroupId = await _abyssRepository.GetGroupIdAsync(abyssContext.TelegramChatId),
            Attempt = 0
        });
        
        await _tBot.DeleteMessageAsync(new DeleteMessageParameters
        {
            ChatId = abyssContext.TelegramChatId,
            MessageId = abyssContext.TelegramMessageId
        });
        
        _logger.LogInformation("Content sent for processing. Session id: {SessionId}", abyssContext.SessionId);
    }

    public async Task SaveContentAsync(Content content)
    {
        Content? contentFromDb = await _abyssRepository.GetContentAsync(content.OriginalLink);
        long chatId = await _abyssRepository.GetChatIdAsync(content.GroupId);
        
        if (contentFromDb is not null)
        {
            List<string> names = new List<string>();

            foreach (Guid userId in contentFromDb.UserIds)
            {
                names.Add(await _abyssRepository.GetUserNameAsync(userId));
            }

            await _tBot.SendMessageAsync(new SendMessageParameters
            {
                ChatId = chatId,
                Text = $"{await _abyssRepository.GetTelegramUserNameAsync(content.UserIds.First())} posted the same content as\n" +
                       $"{string.Join(" and ", names)} 😁👍",
                ReplyToMessageId = contentFromDb.MetaContent.TelegramMessageId ?? default
            });

            await _abyssRepository.AddUserToContentAsync(contentFromDb.Id, content);
            _dataLakeService.RemoveFile(Path.Combine(Constants.ContentFolder, $"{content.Id}.mp4"));
            return;
        }

        await _abyssRepository.SaveContentAsync(content);
        await SendVideoToTelegramAsync(content, chatId);
    }

    private async Task SendVideoToTelegramAsync(Content content, long chatId)
    {
        MemoryStream contentStream = new MemoryStream(
            await _dataLakeService.ReadFileAsync(Path.Combine(Constants.ContentFolder, $"{content.Id}.mp4")));
        
        HttpResponseMessage httpResponseMessage = await _tBot.SendVideoAsync(new SendVideoParameters
        {
            Caption = $"🥰 Content {await _abyssRepository.GetContentCountAsync()}\n" +
                      $"👾 {content.Id}\n" +
                      $"{await _abyssRepository.GetFunnyPhraseAsync(content.UserIds.First())}\n" +
                      $"🥑 {content.OriginalLink}",
            Video = new VideoFile(contentStream, content.Id.ToString()),
            ChatId = chatId
        });
        
        JToken token = JToken.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
        await _abyssRepository.SaveTelegramMessageIdAsync(content.Id, token["result"]?["message_id"]?.ToString().ToInt() ?? default);
    }
}