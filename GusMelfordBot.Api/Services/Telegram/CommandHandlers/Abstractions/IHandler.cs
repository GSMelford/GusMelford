﻿using GusMelfordBot.Domain.Telegram;

namespace GusMelfordBot.Api.Services.Telegram.CommandHandlers.Abstractions;

public interface IHandler
{
    IHandler SetNext(IHandler handler);
    Task<TelegramCommand> HandleAsync(TelegramCommand telegramCommand);
}