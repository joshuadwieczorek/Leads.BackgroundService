using Leads.BackgroundService.Data.Models;
using System.Threading.Tasks;

namespace Leads.BackgroundService.Repositories
{
    public interface IEmailLogRepository
    {
        Task Create(EmailLog emailLog);
    }
}