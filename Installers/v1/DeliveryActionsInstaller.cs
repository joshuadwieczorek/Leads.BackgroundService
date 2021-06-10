using Leads.BackgroundService.DeliveryActions.v1;
using Leads.Domain.Contracts.v1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Leads.BackgroundService.Installers.v1
{
    public class DeliveryActionsInstaller : IInstaller
    {
        public void InstallServices(
              IServiceCollection services
            , IConfiguration configuration)
        {
            services.AddTransient<IAdfDeliveryAction, AdfDeliveryAction>();
            services.AddTransient<IEmailDeliveryAction, EmailDeliveryAction>();
            services.AddTransient<IDotDigitalDeliveryAction, DotDigitalDeliveryAction>();
        }
    }
}