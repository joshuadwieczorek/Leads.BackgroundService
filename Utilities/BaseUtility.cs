using Microsoft.Extensions.Logging;

namespace Leads.BackgroundService.Utilities
{
    public abstract class BaseUtility<T>
    {
        protected readonly ILogger<T> logger;
        protected readonly Bugsnag.IClient bugSnag;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        public BaseUtility(
              ILogger<T> logger
            , Bugsnag.IClient bugSnag)
        {
            this.logger = logger;
            this.bugSnag = bugSnag;
        }
    }
}