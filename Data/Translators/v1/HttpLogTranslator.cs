using Leads.BackgroundService.Data.Models;
using Leads.Domain.Models.v1;

namespace Leads.BackgroundService.Data.Translators.v1
{
    public class HttpLogTranslator
    {
        /// <summary>
        /// Translate http log model to http log.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public HttpLog Translate(HttpLogModel httpLog)
            => new HttpLog
            {
                IpAddress = httpLog?.IpAddress,
                Url = httpLog?.Url,
                StatusCode = httpLog.StatusCode,
                StartTime = httpLog?.StartTime,
                EndTime = httpLog?.EndTime,
                MethodId = httpLog.HttpMethod
            };


        /// <summary>
        /// Translate http log model to http log.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public HttpLogModel Translate(HttpLog httpLog)
            => new HttpLogModel
            {
                LogId = httpLog.LogId,
                IpAddress = httpLog?.IpAddress,
                Url = httpLog?.Url,
                StatusCode = httpLog.StatusCode,
                StartTime = httpLog?.StartTime,
                EndTime = httpLog?.EndTime,
                CreatedAt = httpLog?.CreatedAt,
                CreatedBy = httpLog?.CreatedBy,
                UpdatedAt = httpLog?.UpdatedAt,
                UpdatedBy = httpLog?.UpdatedBy,
                HttpMethod = httpLog.MethodId
            };


        /// <summary>
        /// Translate http log data model.
        /// </summary>
        /// <param name="httpLogData"></param>
        /// <param name="logId"></param>
        /// <returns></returns>
        public HttpLogData Translate(HttpLogDataModel httpLogData, long? logId)
            => new HttpLogData
            {
                LogId = logId,
                Data = httpLogData.Data,
                LogDataTypeId = (int)httpLogData.LogDataType,
            };
    }
}