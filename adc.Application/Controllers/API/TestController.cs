using adc.Dal;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

using adc.Application.Core;
using adc.Services.Interfaces;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace adc.Application.Controllers.API {
    [Route("api/[controller]")]
    public class TestController : BaseController {

        #region Variables

        private readonly IConversionService _conversionService;

        #endregion

        public TestController(IServiceProvider serviceProvider) : base(serviceProvider) {
            _conversionService = ServiceProvider.GetRequiredService<IConversionService>();
        }

        // GET: api/test
        [HttpGet]
        public ActionResult Get() {
            return Ok(_conversionService.GetSingleNoTracking(x => true));
        }
    }
}
