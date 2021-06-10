using Leads.BackgroundService.Repositories;
using Microsoft.Extensions.Logging;

namespace Leads.BackgroundService.DeliveryActions.v1
{
    public abstract class BaseDeliveryAction<T>
    {
        protected readonly object threadLock;
        protected readonly ILogger<T> logger;
        protected readonly Bugsnag.IClient bugSnag;
        protected readonly IQueueRepository queueRepository;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        public BaseDeliveryAction(
              ILogger<T> logger
            , Bugsnag.IClient bugSnag
            , IQueueRepository queueRepository)
        {
            this.threadLock = new object();
            this.logger = logger;
            this.bugSnag = bugSnag;
            this.queueRepository = queueRepository;
        }
    }
}