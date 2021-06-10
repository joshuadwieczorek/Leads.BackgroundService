using Leads.BackgroundService.Repositories;
using Leads.Domain.Contracts.v1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Leads.BackgroundService.Installers.v1
{
    public class RepositoriesInstaller : IInstaller
    {
        public void InstallServices(
              IServiceCollection services
            , IConfiguration configuration)
        {
            services.AddScoped<ILeadProviderRepository, LeadProviderRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IHttpLogRepository, HttpLogRepository>();
            services.AddScoped<IQueueRepository, QueueRepository>();
            services.AddScoped<IAdfRepository, AdfRepository>();
            services.AddScoped<IEmailLogRepository, EmailLogRepository>();
        }
    }
}