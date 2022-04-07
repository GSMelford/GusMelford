using System.Threading.Tasks;

namespace GusMelfordBot.Core.Services.Update;

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Dto.UpdateModule;
using Applications;

public class UpdateService : IUpdateService
{
    private readonly IApplicationService _applicationService;
    private readonly ILogger<UpdateService> _logger;

    public UpdateService(
        IApplicationService applicationService,
        ILogger<UpdateService> logger)
    {
        _applicationService = applicationService;
        _logger = logger;
    }

    public async Task<bool> ProcessUpdate(string json)
    {
        var updateEntity = JsonConvert.DeserializeObject<Update>(json);
        _logger.LogInformation("Update: {Text}", updateEntity?.Message?.Text);
        
        if (updateEntity?.Message is not null)
        {
            await _applicationService.DefineApplicationFromMessage(updateEntity.Message);
        }

        if (updateEntity?.CallbackQuery is not null)
        {
            await _applicationService.DefineApplicationFromCallbackQuery(updateEntity.CallbackQuery);
        }

        return true;
    }
}