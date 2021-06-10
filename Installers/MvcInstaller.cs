using Bugsnag.AspNet.Core;
using Leads.Domain.Contracts.v1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Leads.BackgroundService.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(
              IServiceCollection services
            , IConfiguration configuration)
        {           
            // Add logging.
            services.AddLogging(config =>
            {
                config.AddDebug();
                config.AddConsole();
            });

            // Add Bug Snag.
            services.AddBugsnag(cfg => {
                cfg.ApiKey = configuration["BugsnagApiKey"];
                cfg.AppType = configuration["AppType"];
                cfg.AppVersion = configuration["AppReleaseVersion"];
                cfg.ReleaseStage = configuration["AppReleaseStage"];
            });

            // Add Swagger.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Leads.BackgroundService", Version = "v1" });
            });

            // Add newtonsoft.
            services.AddControllers()
                .AddNewtonsoftJson(c =>
                {
                    c.SerializerSettings.Converters.Add(new StringEnumConverter());
                    c.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });
        }
    }
}