using System;
using Microsoft.Extensions.Logging;
using Leads.BackgroundService.Data;
using Polly;
using Microsoft.Extensions.Configuration;
using AAG.Global.ExtensionMethods;

namespace Leads.BackgroundService.Repositories
{
    public abstract class BaseRepository<T>
    {
        protected readonly object threadLock;
        protected readonly ILogger<T> logger;
        protected readonly Bugsnag.IClient bugSnag;
        protected readonly LeadsDbContext dbContext;
        protected readonly Polly.Retry.AsyncRetryPolicy retryPolicy;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        public BaseRepository(
              IConfiguration configuration
            , ILogger<T> logger
            , Bugsnag.IClient bugSnag
            , LeadsDbContext dbContext)
        {
            this.threadLock = new object();
            this.logger = logger;
            this.bugSnag = bugSnag;
            this.dbContext = dbContext;

            var maxRetryAttempts = configuration["RetryPolicyMaxAttemptsMillisecods"].ToInt(3);    

            retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(maxRetryAttempts, i => TimeSpan.FromMilliseconds(i * 1000));
        }
    }
}