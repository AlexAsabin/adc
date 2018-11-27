using Microsoft.AspNetCore.Mvc;
using System;

namespace adc.Application.Core {
    //[Authorize]
    public class BaseController : Controller {
        protected IServiceProvider ServiceProvider;

        protected BaseController(IServiceProvider serviceProvider) {
            ServiceProvider = serviceProvider;
        }

        protected string GetMessage() {
            return string.Join(Environment.NewLine,
              $"User: {User?.Identity?.Name}",
              $"Path: {Request.Path}",
              $"Method: {Request.Method}"
            );
        }
    }
}
