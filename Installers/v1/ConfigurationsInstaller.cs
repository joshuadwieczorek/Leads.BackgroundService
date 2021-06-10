using Leads.Domain.Contracts.v1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Leads.BackgroundService.Installers.v1
{
    public class ConfigurationsInstaller : IInstaller
    {
        public void InstallServices(
              IServiceCollection services
            , IConfiguration configuration)
        {
            EmailConfiguration emailConfiguration = new EmailConfiguration();
            configuration.Bind("EmailConfiguration", emailConfiguration);            
            services.AddSingleton<EmailConfiguration>(emailConfiguration);
        }
    }
}