using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Leads.BackgroundService.Repositories;
using Leads.Domain.Models.v1;
using Leads.BackgroundService.Data.Models;

namespace Leads.BackgroundService.DeliveryActions.v1
{
    internal class DotDigitalDeliveryAction : BaseDeliveryAction<DotDigitalDeliveryAction>, IDotDigitalDeliveryAction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        public DotDigitalDeliveryAction(
              ILogger<DotDigitalDeliveryAction> logger
            , Bugsnag.IClient bugSnag
            , IQueueRepository queueRepository) : base(logger, bugSnag, queueRepository)
        {

        }


        /// <summary>
        /// Deliver.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="queueItem"></param>
        /// <returns></returns>
        public async Task Deliver(Queue queue, QueueModel queueItem)
        {
            try
            {
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                bugSnag.Notify(e);
                logger.LogError("{e}", e);
                throw;
            }
        }
    }
}