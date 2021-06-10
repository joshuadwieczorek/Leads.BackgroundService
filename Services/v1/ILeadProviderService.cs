using Leads.Domain.Models.v1;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leads.BackgroundService.Services.v1
{
    public interface ILeadProviderService
    {
        Task Delete(int id);
        Task<IEnumerable<LeadProviderModel>> Get();
        Task<LeadProviderModel> Get(int id);
        Task<LeadProviderModel> Get(string name);
        Task<LeadProviderModel> Post(LeadProviderModel leadProviderModel);
    }
}