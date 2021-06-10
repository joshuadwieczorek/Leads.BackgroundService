using Microsoft.Extensions.Logging;

namespace Leads.BackgroundService.Services
{
    public abstract class BaseService<T>
    {
        protected readonly object threadLock;
        protected readonly ILogger<T> logger;
        protected readonly Bugsnag.IClient bugSnag;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        public BaseService(
              ILogger<T> logger
            , Bugsnag.IClient bugSnag)
        {
            this.threadLock = new object();
            this.logger = logger;
            this.bugSnag = bugSnag;
        }
    }
}