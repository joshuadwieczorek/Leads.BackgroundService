using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Leads.BackgroundService.Services.v1
{
    public class HostedService : IHostedService
    {
        private readonly ILogger<HostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly CancellationToken _cancellationToken;
        private readonly int _maxQueueItemsToProcess;


        public HostedService(
              ILogger<HostedService> logger
            , IServiceProvider serviceProvider
            , IHostApplicationLifetime applicationLifetime
            , IConfiguration configuration
            )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _cancellationToken = applicationLifetime.ApplicationStopping;

            if (!int.TryParse(configuration["MaxQueueItemsToProcess"], out _maxQueueItemsToProcess))
                _maxQueueItemsToProcess = 1;
        }


        /// <summary>
        /// Start the hosted background service.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                Bugsnag.IClient bugSnag = scope.ServiceProvider.GetRequiredService<Bugsnag.IClient>();
                IQueueProcessService service = scope.ServiceProvider.GetRequiredService<IQueueProcessService>();

                while (!_cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await service.Process(_maxQueueItemsToProcess);
                        await Task.Delay((1000 * 60 * 1));
                    }
                    catch (Exception e)
                    {
                        bugSnag.Notify(e);
                        _logger.LogError("{e}", e);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("{e}", e);
            }
        }


        /// <summary>
        /// On hosted background service stop.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}