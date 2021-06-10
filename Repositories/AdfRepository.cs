using System;
using System.Threading.Tasks;
using Leads.BackgroundService.Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Leads.BackgroundService.Data;

namespace Leads.BackgroundService.Repositories
{
    public class AdfRepository : BaseRepository<AdfRepository>, IAdfRepository
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        public AdfRepository(
              IConfiguration configuration
            , ILogger<AdfRepository> logger
            , Bugsnag.IClient bugSnag
            , LeadsDbContext dbContext) : base(configuration, logger, bugSnag, dbContext) { }


        /// <summary>
        /// Create new adf in the database.
        /// </summary>
        /// <param name="adf"></param>
        /// <returns></returns>
        public async Task Create(Adf adf)
        {
            await retryPolicy.ExecuteAsync(async () => 
            {
                adf.CreatedAt = DateTime.Now;
                adf.CreatedBy = Environment.UserName;
                dbContext.Adfs.Add(adf);
                await dbContext.SaveChangesAsync();
            });            
        }
    }
}