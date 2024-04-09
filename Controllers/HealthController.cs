using Microsoft.AspNetCore.Mvc;

namespace sampleapi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet(Name = "health")]
        public IActionResult Get()
        {
            return new JsonResult(new { status = "OK" });
        }
    }
}

