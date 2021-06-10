using System.Threading.Tasks;

namespace Leads.BackgroundService.Services.v1
{
    public interface IQueueProcessService
    {
        Task Process(int maxQueueItemsToProcess);
    }
}