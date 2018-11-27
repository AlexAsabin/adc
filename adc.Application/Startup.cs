using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.Extensions.Configuration;
using adc.Dal;
using Microsoft.EntityFrameworkCore;

namespace adc.Application {
  public class Startup {
    public Startup(IHostingEnvironment env) {
      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
      builder.AddEnvironmentVariables();
      Configuration = builder.Build();
    }

    public void ConfigureServices(IServiceCollection services) {
      services.AddDbContext<AdcDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ADC")));
      services.AddMvc();
    }

    public IConfiguration Configuration { get; }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
      app.Use(async (context, next) => {
        await next();
        if (context.Response.StatusCode == 404 &&
            !Path.HasExtension(context.Request.Path.Value) &&
            !context.Request.Path.Value.StartsWith("/api/")) {
          context.Request.Path = "/index.html";
          await next();
        }
      });
      app.UseMvcWithDefaultRoute();
      app.UseDefaultFiles();
      app.UseStaticFiles();
    }
  }
}
