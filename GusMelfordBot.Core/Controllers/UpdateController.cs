using Newtonsoft.Json.Linq;

namespace GusMelfordBot.Core.Controllers
{
    using System;
    using Newtonsoft.Json;
    using Services.Update;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Telegram.Dto.UpdateModule;
    
    [ApiController]
    public class UpdateController : Controller
    {
        private readonly ILogger<UpdateController> _logger;
        private readonly IUpdateService _updateService;
        
        public UpdateController(
            ILogger<UpdateController> logger, 
            IUpdateService updateService)
        {
            _logger = logger;
            _updateService = updateService;
        }

        [HttpPost("update")]
        public IActionResult Update([FromBody]object update)
        {
            string content = update?.ToString();
            if (string.IsNullOrEmpty(content))
            {
                return Ok();
            }
            
            var updateEntity = JsonConvert.DeserializeObject<Update>(content);
            JToken token = JToken.Parse(content);

            if (updateEntity?.Message is not null)
            {
                JToken replayToMessage = JToken.Parse(content)?["message"]?["reply_to_message"];
                if (replayToMessage is not null)
                {
                    updateEntity.Message.ReplyToMessage =
                        JsonConvert.DeserializeObject<Message>(replayToMessage.ToString());
                }
            }
            
            _logger.LogInformation("Update. Token: {Token}, Body: {@UpdateText}", token, update);
            try {
                _updateService.ProcessUpdate(updateEntity);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "ProcessUpdate error. UpdateId: {UpdateId}", updateEntity?.UpdateId);
            }
            return Ok();
        }
    }
}