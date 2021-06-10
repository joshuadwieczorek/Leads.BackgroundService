using System;
using System.Linq;
using System.Threading.Tasks;
using Leads.BackgroundService.Data.Models;
using Microsoft.Extensions.Logging;
using Leads.BackgroundService.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Leads.BackgroundService.Repositories
{
    public class QueueRepository : BaseRepository<QueueRepository>, IQueueRepository
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        public QueueRepository(
              IConfiguration configuration
            , ILogger<QueueRepository> logger
            , Bugsnag.IClient bugSnag
            , LeadsDbContext dbContext) : base(configuration, logger, bugSnag, dbContext) { }


        /// <summary>
        /// Queue item in the database.
        /// </summary>
        /// <param name="queueItem"></param>
        /// <returns></returns>
        public async Task QueueItem(Queue queueItem)
        {
            await retryPolicy.ExecuteAsync(async () => 
            {
                queueItem.CreatedAt = DateTime.Now;
                queueItem.CreatedBy = Environment.UserName;
                queueItem.QueuedAt = DateTime.Now;
                queueItem.QueueStatusId = 1;
                queueItem.ProcessAttempts = 0;
                dbContext.Queue.Add(queueItem);
                await dbContext.SaveChangesAsync();
            });
        }


        /// <summary>
        /// Read queue items.
        /// </summary>
        /// <param name="maxQueueItemsToProcess"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Queue>> ReadQueue(int maxQueueItemsToProcess)
        {
            return await retryPolicy.ExecuteAsync<IEnumerable<Queue>>(async () =>
            {
                IEnumerable<Queue> items = await
                    dbContext
                    .Queue
                    .Where(q => (q.QueueStatusId == 1 || q.QueueStatusId == 5))
                    .OrderBy(q => q.QueuedAt)
                    .Take(maxQueueItemsToProcess)
                    .ToListAsync();

                foreach (var item in items)
                {
                    item.QueueStatusId = item.QueueStatusId + 1;
                    item.PickedUpAt = DateTime.Now;
                    item.UpdatedAt = DateTime.Now;
                    item.UpdatedBy = Environment.UserName;
                }

                await dbContext.SaveChangesAsync();

                return items;
            });
        }

        
        /// <summary>
        /// Update queue status id.
        /// </summary>
        /// <param name="queueId"></param>
        /// <param name="queueStatusId"></param>
        /// <returns></returns>
        public async Task UpdateStatusId(
               long queueId
             , int queueStatusId)
        {
            await retryPolicy.ExecuteAsync(async () =>
            {
                var queueItem = await dbContext.Queue.FindAsync(queueId);
                queueItem.QueueStatusId = queueStatusId;
                queueItem.ProcessAttempts = 1;
                queueItem.ProcessedAt = DateTime.Now;
                queueItem.UpdatedAt = DateTime.Now;
                queueItem.UpdatedBy = Environment.UserName;
                await dbContext.SaveChangesAsync();
            });
        }
    }
}