using GusMelfordBot.Domain;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Events;
using GusMelfordBot.Extensions.Services.DataLake;
using GusMelfordBot.SimpleKafka.Interfaces;
using TBot.Client;
using TBot.Client.Api.Telegram.DeleteMessage;
using TBot.Client.Api.Telegram.SendMessage;
using TBot.Client.Api.Telegram.SendVideo;

namespace GusMelfordBot.Api.Services.Applications.ContentCollector;

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
            UserId = userId.ToString()!,
            GroupId = (await _abyssRepository.GetGroupIdAsync(abyssContext.TelegramChatId)).ToString(),
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
            await _tBot.SendMessageAsync(new SendMessageParameters
            {
                ChatId = chatId,
                Text = "Oops, {NAME} posted the same content 😁👍"
            });

            await _abyssRepository.AddUserToContentAsync(contentFromDb.Id, content.UserIds.FirstOrDefault());
            return;
        }

        await _abyssRepository.SaveContentAsync(content);
        
        MemoryStream contentStream = new MemoryStream(
            await _dataLakeService.ReadFileAsync(Path.Combine(Constants.ContentFolder, $"{content.Id}.mp4")));
        
        //TODO Saved message id
        HttpResponseMessage httpResponseMessage = await _tBot.SendVideoAsync(new SendVideoParameters
        {
            Caption = $"Content № {await _abyssRepository.GetContentCountAsync()}",
            Video = new VideoFile(contentStream, content.Id.ToString()),
            ChatId = chatId
        });
    }
}