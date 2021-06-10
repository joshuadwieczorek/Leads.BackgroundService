using Leads.BackgroundService.Data.Models;
using System.Threading.Tasks;

namespace Leads.BackgroundService.Repositories
{
    public interface IHttpLogRepository
    {
        Task<HttpLog> Create(HttpLog httpLog);
        Task Create(HttpLogData logData);
        Task<HttpLog> Update(long logId, HttpLog httpLog);
    }
}