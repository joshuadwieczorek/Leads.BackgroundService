using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Leads.BackgroundService.Repositories;
using Leads.BackgroundService.Data.Models;
using Leads.BackgroundService.Data.Translators.v1;
using System.Collections.Generic;
using Leads.Domain.Models.v1;
using System.Linq;
using Leads.BackgroundService.DeliveryActions.v1;
using Microsoft.Extensions.DependencyInjection;
using Leads.BackgroundService.Helpers;

namespace Leads.BackgroundService.Services.v1
{
    public class QueueProcessService : BaseService<QueueProcessService>, IQueueProcessService
    {
        protected readonly object threadLock;
        protected readonly ILogger<QueueProcessService> logger;
        protected readonly Bugsnag.IClient bugSnag;
        private readonly IServiceProvider _serviceProvider;
        private readonly IQueueRepository _queueRepository;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        /// <param name="leadProviderRepository"></param>
        public QueueProcessService(
              ILogger<QueueProcessService> logger
            , Bugsnag.IClient bugSnag
            , IServiceProvider serviceProvider
            , IQueueRepository queueRepository) : base(logger, bugSnag)
        {
            _serviceProvider = serviceProvider;
            _queueRepository = queueRepository;
        }


        /// <summary>
        /// Process the queue.
        /// </summary>
        /// <returns></returns>
        public async Task Process(int maxQueueItemsToProcess)
        {
            try
            {
                List<Task> tasks = new List<Task>();
                var queueItems = await _queueRepository.ReadQueue(maxQueueItemsToProcess);

                foreach (var queueItem in queueItems)
                    tasks.Add(Process(queueItem));

                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                bugSnag.Notify(e);
                logger.LogError("{e}", e);
            }
        }


        /// <summary>
        /// Process single queue item.
        /// </summary>
        /// <param name="queueItem"></param>
        /// <returns></returns>
        private async Task Process(Queue queueItem)
        {
            try
            {
                // queue model.
                var queueModel = new QueueTranslator()
                    .Translate(queueItem);

                // validate queue model.
                ValidateQueueItem(queueModel);

                // Set delivery action processor to null.
                IDeliveryAction deliveryActionProcessor = null;

                // Process via delivery actions.
                foreach (var deliveryAction in queueModel.Token.Configuration.DeliveryActions)
                {
                    try
                    {
                        // Set delivery action.
                        deliveryActionProcessor = deliveryAction switch
                        {
                            Domain.Enums.DeliveryAction.Adf => _serviceProvider.GetRequiredService<IAdfDeliveryAction>(),
                            Domain.Enums.DeliveryAction.DotDigital => _serviceProvider.GetRequiredService<IDotDigitalDeliveryAction>(),
                            Domain.Enums.DeliveryAction.Email => _serviceProvider.GetRequiredService<IEmailDeliveryAction>(),
                            _ => throw new ArgumentException($"No delivery action found for '{deliveryAction}'")
                        };

                        // Process delivery.
                        if (deliveryActionProcessor is not null)
                        {
                            await deliveryActionProcessor.Deliver(queueItem, queueModel);
                            await _queueRepository.UpdateStatusId(queueItem.QueueId, QueueStatusIdHelper.Calculate(queueItem.QueueStatusId));
                        }
                    }
                    catch (Exception e)
                    {
                        bugSnag.Notify(e);
                        logger.LogError("{e}", e);
                        await _queueRepository.UpdateStatusId(queueItem.QueueId, QueueStatusIdHelper.Calculate(queueItem.QueueStatusId, true));
                    }
                }
            }
            catch (Exception e)
            {
                bugSnag.Notify(e);
                logger.LogError("{e}", e);
            }
        }


        /// <summary>
        /// Validate queue item.
        /// </summary>
        /// <param name="queueItem"></param>
        private void ValidateQueueItem(QueueModel queueItem)
        {
            if (queueItem is null)
                throw new ArgumentException(nameof(queueItem));

            if (queueItem.Token is null)
                throw new ArgumentException($"{nameof(queueItem)}: Token is null");

            if (queueItem.Token.Configuration is null)
                throw new ArgumentException($"{nameof(queueItem)}: Token.Configuration is null");

            if (queueItem.Token.Configuration.DeliveryActions is null || !queueItem.Token.Configuration.DeliveryActions.Any())
                throw new ArgumentException($"{nameof(queueItem)}: Token.Configuration.DeliveryActions is null or Token.Configuration.DeliveryActions is empty");

            if (queueItem.LeadInformation is null)
                throw new ArgumentException($"{nameof(queueItem)}: LeadInformation is null");           
        }
    }
}