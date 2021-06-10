using Leads.BackgroundService.Data.Models;
using System.Threading.Tasks;

namespace Leads.BackgroundService.Repositories
{
    public interface IAdfRepository
    {
        Task Create(Adf adf);
    }
}