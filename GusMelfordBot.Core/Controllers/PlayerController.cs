namespace GusMelfordBot.Core.Controllers
{
    using System.Threading.Tasks;
    using Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    
    [ApiController]
    [Route("player")]
    public class PlayerController : Controller
    {
        private readonly ILogger<PlayerController> _logger;
        private readonly IPlayerService _playerService;
        
        public PlayerController(
            ILogger<PlayerController> logger,
            IPlayerService playerService)
        {
            _logger = logger;
            _playerService = playerService;
        }

        [HttpGet("video/new/next")]
        public async Task<FileStreamResult> GetNextVideoStream()
        {
            _logger.LogInformation("{PlayerController} - {Query}", 
                nameof(PlayerController), HttpContext.Request.Path);
            
            return await _playerService.GetNextVideoStream(x=> !x.IsViewed);
        }
    }
}