using Leads.BackgroundService.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leads.BackgroundService.Repositories
{
    public interface ILeadProviderRepository
    {
        Task<LeadProvider> Create(LeadProvider provider);
        Task Delete(int id);
        Task<IEnumerable<LeadProvider>> Read();
        Task<LeadProvider> Read(int id);
        Task<LeadProvider> Read(string name);
    }
}