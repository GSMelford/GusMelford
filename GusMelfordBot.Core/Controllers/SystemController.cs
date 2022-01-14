using System.Threading.Tasks;

namespace GusMelfordBot.Core.Controllers
{
    using Interfaces;
    using Microsoft.AspNetCore.Mvc;
    
    [ApiController]
    public class GusMelfordBotController : Controller
    {
        private readonly ISystemService _systemService;
        
        public GusMelfordBotController(ISystemService systemService)
        {
            _systemService = systemService;
        }
        
        [HttpGet("info")]
        public async Task<JsonResult> GetSystemInfo()
        {
            return Json(await _systemService.GetSystemData());
        }
    }
}