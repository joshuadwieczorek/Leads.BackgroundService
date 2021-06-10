using System;
using System.Threading.Tasks;
using Leads.BackgroundService.Data.Models;
using Microsoft.Extensions.Logging;
using Leads.BackgroundService.Data;
using Microsoft.Extensions.Configuration;

namespace Leads.BackgroundService.Repositories
{
    public class EmailLogRepository : BaseRepository<EmailLogRepository>, IEmailLogRepository
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        public EmailLogRepository(
              IConfiguration configuration
            , ILogger<EmailLogRepository> logger
            , Bugsnag.IClient bugSnag
            , LeadsDbContext dbContext) : base(configuration, logger, bugSnag, dbContext) { }


        /// <summary>
        /// Create new email log in the database.
        /// </summary>
        /// <param name="emailLog"></param>
        /// <returns></returns>
        public async Task Create(EmailLog emailLog)
        {
            await retryPolicy.ExecuteAsync(async () => 
            {
                emailLog.CreatedAt = DateTime.Now;
                emailLog.CreatedBy = Environment.UserName;
                dbContext.EmailLog.Add(emailLog);
                await dbContext.SaveChangesAsync();
            });
        }
    }
}