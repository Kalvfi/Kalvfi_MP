using Microsoft.AspNetCore.Mvc;

namespace WorksheetGen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public ActionResult Get() => Ok(new { status = "ok" });
    }
}
