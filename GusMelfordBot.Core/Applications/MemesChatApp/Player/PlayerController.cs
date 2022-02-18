using GusMelfordBot.Core.Applications.MemesChatApp.Interfaces;

namespace GusMelfordBot.Core.Applications.MemesChatApp.Player
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    
    [ApiController]
    [Route("player/video")]
    public class PlayerController : Controller
    {
        private readonly ILogger<PlayerController> _logger;
        private readonly IPlayerService _playerService;
        
        private const string CONTENT_TYPE = "video/mp4";
        
        public PlayerController(
            ILogger<PlayerController> logger,
            IPlayerService playerService)
        {
            _logger = logger;
            _playerService = playerService;
        }
        
        [HttpGet("reset")]
        public IActionResult Reset()
        {
            _playerService.Reset();
            return Ok();
        }
        
        [HttpGet("next")]
        public async Task<JsonResult> GetNextVideoStream()
        {
            return Json(await _playerService.SetNextVideo());
        }
        
        [HttpGet("prev")]
        public async Task<JsonResult> GetPreviousVideoStream()
        {
            return Json(await _playerService.SetPreviousVideo());
        }
        
        [HttpGet("current")]
        public FileStreamResult GetCurrentVideo([FromQuery] string updated)
        {
            FileStreamResult fileStreamResult =
                new FileStreamResult(new MemoryStream(_playerService.CurrentContentBytes), CONTENT_TYPE);
            
            HttpContext.Response.Headers.Add("Content-Length", _playerService.CurrentContentBytes.Length.ToString());
            HttpContext.Response.Headers.Add("Accept-Ranges", "bytes");
            HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Response.Headers.Add("Pragma", "no-cache");
            HttpContext.Response.Headers.Add("Expires", "0");
           
            _logger.LogInformation("Update to new video. Request time: {Updated}", updated);
            return fileStreamResult;
        }
    }
}