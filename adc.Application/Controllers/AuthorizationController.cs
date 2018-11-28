using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using adc.Application.Core;
using adc.Dtos.Security;
using adc.Entities.Security;
using adc.Services.Base;

namespace Start.Application.Controllers {
    [Route("api/[controller]")]
    public class AuthorizationController : BaseController {
        public AuthorizationController(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        [HttpGet]
        public IActionResult Get() {
            var service = ServiceProvider.GetRequiredService<IBaseService<User, UserModel>>();
            var includes = service.CreateIncludes(x => x.UserRoles.Select(y => y.Role));
            return Ok(service.GetSingle(x => x.Email.ToLower().Equals(User.Identity.Name.ToLower()), includes));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model) {
            var service = ServiceProvider.GetRequiredService<adc.Services.Interfaces.Core.IAuthorizationService>();
            try {
                var result = await service.Login(model);
                return Ok(result);
            } catch (Exception e) {
                return StatusCode((int)HttpStatusCode.ExpectationFailed, e.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Logout() {
            var service = ServiceProvider.GetRequiredService<adc.Services.Interfaces.Core.IAuthorizationService>();
            await service.Logout();
            return NoContent();
        }
    }
}
