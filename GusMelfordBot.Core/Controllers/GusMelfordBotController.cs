namespace GusMelfordBot.Core.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    
    [ApiController]
    public class GusMelfordBotController : Controller
    {
        private readonly IGusMelfordBotService _gusMelfordBotService;
        private readonly IDataService _dataService;
        private readonly ILogger<GusMelfordBotController> _logger;
        
        public GusMelfordBotController(
            ILogger<GusMelfordBotController> logger, 
            IGusMelfordBotService gusMelfordBotService,
            ITikTokService tikTokService,
            IDataService dataService)
        {
            _dataService = dataService;
            _gusMelfordBotService = gusMelfordBotService;
            _logger = logger;
        }

        [HttpGet("start")]
        public ActionResult<string> Start()
        {
            _gusMelfordBotService.StartListenUpdate();
            _logger.LogInformation("GusMelfordBot started listen update Time: {Time}", DateTime.UtcNow);
            return Ok();
        }

        [HttpGet("video/new")]
        public async Task<JsonResult> GetUnwatchVideo()
        {
            return Json(await _dataService.GetUnwatchTikTokVideo());
        }
        
        [HttpGet("video")]
        public async Task<JsonResult> GetUnwatchVideo(
            [FromQuery] string takeDateSince, 
            [FromQuery] string takeDateUntil)
        {
            return Json(await _dataService.GetTikTokVideo(takeDateSince, takeDateUntil));
        }
        
        [HttpGet("systemData")]
        public JsonResult GetSystemData()
        {
            return Json(_dataService.GetSystemData());
        }
    }
}