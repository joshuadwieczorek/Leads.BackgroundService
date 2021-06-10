using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leads.BackgroundService.Data.Models;
using Microsoft.Extensions.Logging;
using Leads.BackgroundService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Leads.BackgroundService.Repositories
{
    public class LeadProviderRepository : BaseRepository<LeadProviderRepository>, ILeadProviderRepository
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        public LeadProviderRepository(
              IConfiguration configuration
            , ILogger<LeadProviderRepository> logger
            , Bugsnag.IClient bugSnag
            , LeadsDbContext dbContext) : base(configuration, logger, bugSnag, dbContext) { }


        /// <summary>
        /// Create new lead provider.
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public async Task<LeadProvider> Create(LeadProvider provider)
        {
            return await retryPolicy.ExecuteAsync<LeadProvider>(async () => 
            {
                dbContext.LeadProviders.Add(provider);
                await dbContext.SaveChangesAsync();
                return await Read(provider.Name);
            });
        }


        /// <summary>
        /// Read all lead providers.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<LeadProvider>> Read()
        {
            return await retryPolicy.ExecuteAsync<IEnumerable<LeadProvider>>(async () =>
            {
                return await
                    dbContext
                    .LeadProviders
                    .ToListAsync();
            });
        }


        /// <summary>
        /// Read lead provider by id.
        /// </summary>
        /// <returns></returns>
        public async Task<LeadProvider> Read(int id)
        {
            return await retryPolicy.ExecuteAsync<LeadProvider>(async () =>
            {
                return await
                    dbContext
                    .LeadProviders
                    .Where(p => p.LeadProviderId == id)
                    .FirstOrDefaultAsync();
            });
        }


        /// <summary>
        /// Read lead provider by name.
        /// </summary>
        /// <returns></returns>
        public async Task<LeadProvider> Read(string name)
        {
            return await retryPolicy.ExecuteAsync<LeadProvider>(async () =>
            {
                if (!String.IsNullOrEmpty(name))
                    return await
                    dbContext
                    .LeadProviders
                    .Where(p => p.Name.ToLower() == name.ToLower())
                    .FirstOrDefaultAsync();

                return null;
            });
        }


        /// <summary>
        /// Delete lead provider.
        /// </summary>
        /// <returns></returns>
        public async Task Delete(int id)
        {
            await retryPolicy.ExecuteAsync(async () =>
            {
                var leadProvider = await Read(id);
                if (leadProvider is null)
                    return;
                dbContext.LeadProviders.Remove(leadProvider);
                await dbContext.SaveChangesAsync();
            });
        }
    }
}