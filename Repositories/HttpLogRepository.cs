using System;
using System.Linq;
using System.Threading.Tasks;
using Leads.BackgroundService.Data.Models;
using Microsoft.Extensions.Logging;
using Leads.BackgroundService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Leads.BackgroundService.Repositories
{
    public class HttpLogRepository : BaseRepository<HttpLogRepository>, IHttpLogRepository
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        public HttpLogRepository(
              IConfiguration configuration
            , ILogger<HttpLogRepository> logger
            , Bugsnag.IClient bugSnag
            , LeadsDbContext dbContext) : base(configuration, logger, bugSnag, dbContext) { }


        /// <summary>
        /// Create new http log entry.
        /// </summary>
        /// <param name="httpLog"></param>
        /// <returns></returns>
        public async Task<HttpLog> Create(HttpLog httpLog)
        {
            return await retryPolicy.ExecuteAsync<HttpLog>(async () =>
            {
                httpLog.CreatedAt = DateTime.Now;
                httpLog.CreatedBy = Environment.UserName;
                httpLog.StatusCode = 0;
                httpLog.EndTime = null;
                dbContext.HttpLogs.Add(httpLog);
                await dbContext.SaveChangesAsync();
                return httpLog;
            });
        }


        /// <summary>
        /// Create http log data.
        /// </summary>
        /// <param name="logData"></param>
        /// <returns></returns>
        public async Task Create(HttpLogData logData)
        {
            await retryPolicy.ExecuteAsync(async () =>
            {
                logData.CreatedAt = DateTime.Now;
                logData.CreatedBy = Environment.UserName;
                dbContext.HttpLogData.Add(logData);
                await dbContext.SaveChangesAsync();
            });
        }


        /// <summary>
        /// Update http log entry.
        /// </summary>
        /// <param name="logId"></param>
        /// <param name="httpLog"></param>
        /// <returns></returns>
        public async Task<HttpLog> Update(long logId, HttpLog httpLog)
        {
            return await retryPolicy.ExecuteAsync<HttpLog>(async () =>
            {
                var log = await dbContext
                    .HttpLogs
                    .Where(l => l.LogId == logId)
                    .FirstOrDefaultAsync();

                if (log is null)
                    return null;

                log.StatusCode = httpLog.StatusCode;
                log.EndTime = httpLog.EndTime;
                if (httpLog.IpAddress is not null)
                    log.IpAddress = httpLog.IpAddress;
                if (httpLog.Url is not null)
                    log.Url = httpLog.Url;
                log.UpdatedAt = DateTime.Now;
                log.UpdatedBy = Environment.UserName;
                await dbContext.SaveChangesAsync();
                return httpLog;
            });
        }
    }
}