

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GusMelfordBot.Core.Controllers
{
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
        public IActionResult Update([FromBody]object update, string token)
        {
            var updateEntity = JsonConvert.DeserializeObject<Update>(update?.ToString());
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