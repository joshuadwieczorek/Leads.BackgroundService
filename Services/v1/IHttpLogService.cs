using Leads.Domain.Models.v1;
using System.Threading.Tasks;

namespace Leads.BackgroundService.Services.v1
{
    public interface IHttpLogService
    {
        Task<HttpLogModel> Post(HttpLogModel httpLogModel);
        Task Put(long logId, HttpLogModel httpLogModel);
    }
}