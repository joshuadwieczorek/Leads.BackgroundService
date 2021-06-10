using System.Threading.Tasks;
using Leads.Domain.Models.v1;
using Leads.BackgroundService.Data.Models;

namespace Leads.BackgroundService.DeliveryActions.v1
{
    internal interface IDeliveryAction
    {
        public Task Deliver(Queue queue, QueueModel queueItem);
    }
}