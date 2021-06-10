using Leads.BackgroundService.Services.v1;
using Leads.Domain.Contracts.v1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Leads.BackgroundService.Installers.v1
{
    public class ServicesInstaller : IInstaller
    {
        public void InstallServices(
              IServiceCollection services
            , IConfiguration configuration)
        {
            // Normal services.
            services.AddScoped<ILeadProviderService, LeadProviderService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IHttpLogService, HttpLogService>();
            services.AddScoped<IQueueService, QueueService>();
            services.AddScoped<IQueueProcessService, QueueProcessService>();
        }
    }
}