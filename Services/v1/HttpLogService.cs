using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Leads.BackgroundService.Data.Translators.v1;
using Leads.BackgroundService.Repositories;
using Leads.Domain.Models.v1;

namespace Leads.BackgroundService.Services.v1
{
    public class HttpLogService : BaseService<HttpLogService>, IHttpLogService
    {
        private readonly IHttpLogRepository _httpLogRepository;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        /// <param name="leadProviderRepository"></param>
        public HttpLogService(
              ILogger<HttpLogService> logger
            , Bugsnag.IClient bugSnag
            , IHttpLogRepository httpLogRepository) : base(logger, bugSnag)
        {
            _httpLogRepository = httpLogRepository;
        }


        /// <summary>
        /// Create http log entry.
        /// </summary>
        /// <param name="httpLogModel"></param>
        /// <returns></returns>
        public async Task<HttpLogModel> Post(HttpLogModel httpLogModel)
        {
            try
            {
                var translator = new HttpLogTranslator();
                var httpLog = translator
                    .Translate(httpLogModel);

                var newLog = await _httpLogRepository.Create(httpLog);

                var requestHeaders = translator
                    .Translate(httpLogModel.RequestHeaders, newLog.LogId);

                var requestBody = translator
                    .Translate(httpLogModel.RequestBody, newLog.LogId);

                await _httpLogRepository.Create(requestHeaders);
                await _httpLogRepository.Create(requestBody);

                return translator
                    .Translate(newLog);
            }
            catch (Exception e)
            {
                bugSnag.Notify(e);
                logger.LogError("{e}", e);
                throw;
            }
        }


        /// <summary>
        /// Update log entry.
        /// </summary>
        /// <param name="logId"></param>
        /// <param name="httpLogModel"></param>
        /// <returns></returns>
        public async Task Put(long logId, HttpLogModel httpLogModel)
        {
            try
            {
                var translator = new HttpLogTranslator();
                var httpLog = translator
                    .Translate(httpLogModel);

                await _httpLogRepository.Update(logId, httpLog);

                var responseBody = translator
                    .Translate(httpLogModel.ResponseBody, logId);

                await _httpLogRepository.Create(responseBody);
            }
            catch (Exception e)
            {
                bugSnag.Notify(e);
                logger.LogError("{e}", e);
                throw;
            }
        }
    }
}