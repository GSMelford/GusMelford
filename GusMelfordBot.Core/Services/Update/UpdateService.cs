using System.Threading.Tasks;

namespace GusMelfordBot.Core.Services.Update;

using Applications;
    
public class UpdateService : IUpdateService
{
    private readonly IApplicationService _applicationService;
        
    public UpdateService(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }
        
    public async Task ProcessUpdate(Telegram.Dto.UpdateModule.Update update)
    {
        if(update.Message is not null)
        { 
            await _applicationService.DefineApplicationFromMessage(update.Message);
        }

        if(update.CallbackQuery is not null)
        {
            await _applicationService.DefineApplicationFromCallbackQuery(update.CallbackQuery);
        }
    }
}