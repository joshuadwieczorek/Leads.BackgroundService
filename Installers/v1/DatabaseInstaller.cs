using Leads.BackgroundService.Data;
using Leads.Domain.Contracts.v1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Leads.BackgroundService.Installers.v1
{
    public class DatabaseInstaller : IInstaller
    {
        public void InstallServices(
              IServiceCollection services
            , IConfiguration configuration)
        {
            services.AddDbContext<LeadsDbContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("Default"));
            }, ServiceLifetime.Transient);
        }
    }
}