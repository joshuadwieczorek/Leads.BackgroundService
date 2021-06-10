using Leads.Domain.Models.v1;
using System.Threading.Tasks;

namespace Leads.BackgroundService.Services.v1
{
    public interface IQueueService
    {
        Task Post(QueueModel queueModel);
    }
}