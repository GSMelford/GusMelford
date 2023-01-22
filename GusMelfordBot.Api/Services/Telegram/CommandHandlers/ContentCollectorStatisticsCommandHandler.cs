using System.Text;
using GusMelfordBot.Api.Services.Telegram.CommandHandlers.Abstractions;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Domain.Telegram;
using TBot.Client;
using TBot.Client.Api.Telegram.SendMessage;

namespace GusMelfordBot.Api.Services.Telegram.CommandHandlers;

public class ContentCollectorStatisticsCommandHandler : AbstractCommandHandler
{
    private readonly ITBot _tBot;
    
    public ContentCollectorStatisticsCommandHandler(ITBot tBot)
    {
        _tBot = tBot;
    }

    public override async Task<TelegramCommand> Handle(TelegramCommand telegramCommand)
    {
        if (telegramCommand.Name == Commands.ContentCollectorStatistics)
        {
            ContentCollectorStatistic contentCollectorStatistic = null;
            await _tBot.SendMessageAsync(new SendMessageParameters
            {
                Text = BuildMessage(contentCollectorStatistic),
                ChatId = telegramCommand.ChatId
            });

            return telegramCommand;
        }
        
        return await base.Handle(telegramCommand);
    }

    private string BuildMessage(ContentCollectorStatistic contentCollectorStatistic)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("🥲 Keep your stats dude:\n\n");

        foreach (var userNewContent in contentCollectorStatistic.UserNewContents)
        {
            stringBuilder.Append($"{userNewContent.Key}: {userNewContent.Value}\n");
        }

        stringBuilder.Append($"\n😲 Haven't watched {contentCollectorStatistic.NotViewedVideoCount} content yet\n");
        stringBuilder.Append($"The duration of this is only {contentCollectorStatistic.Duration / 60} minutes ⌛");
        return stringBuilder.ToString();
    }
}