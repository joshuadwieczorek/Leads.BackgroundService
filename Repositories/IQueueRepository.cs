using Leads.BackgroundService.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leads.BackgroundService.Repositories
{
    public interface IQueueRepository
    {
        Task QueueItem(Queue queueItem);
        Task<IEnumerable<Queue>> ReadQueue(int maxQueueItemsToProcess);
        Task UpdateStatusId(long queueId, int queueStatusId);
    }
}