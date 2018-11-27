using Microsoft.AspNetCore.Mvc;

namespace adc.Application.Controllers.API {
    [Route("api/[controller]")]
    public class TestController : Controller {
        // GET: api/values
        [HttpGet]
        public ActionResult Get() {
          return Ok(Json("Weclome, Sir!"));
        }
    }
}
