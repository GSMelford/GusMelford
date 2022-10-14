using GusMelfordBot.Api.Services.Telegram.CommandHandlers;
using GusMelfordBot.Api.Services.Telegram.CommandHandlers.Abstractions;
using GusMelfordBot.Domain.Telegram;

namespace GusMelfordBot.Api.Services.Telegram;

public class CommandService : ICommandService
{
    private readonly IServiceProvider _serviceProvider;
    
    public CommandService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task ExecuteAsync(Command command)
    {
        AbstractCommandHandler abstractCommandHandler = 
            ActivatorUtilities.CreateInstance<UserInfoCommandHandler>(_serviceProvider);
        
        abstractCommandHandler
            .SetNext(ActivatorUtilities.CreateInstance<SetPasswordCommandHandler>(_serviceProvider))
            .SetNext(ActivatorUtilities.CreateInstance<ContentCollectorStatisticsCommandHandler>(_serviceProvider));
        
        await abstractCommandHandler.Handle(command);
    }
}