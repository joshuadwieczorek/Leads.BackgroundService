using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Leads.BackgroundService.Data.Translators.v1;
using Leads.BackgroundService.Repositories;
using Leads.Domain.Models.v1;

namespace Leads.BackgroundService.Services.v1
{
    public class QueueService : BaseService<QueueService>, IQueueService
    {
        private readonly IQueueRepository _queueRepository;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        /// <param name="leadProviderRepository"></param>
        public QueueService(
              ILogger<QueueService> logger
            , Bugsnag.IClient bugSnag
            , IQueueRepository queueRepository) : base(logger, bugSnag)
        {
            _queueRepository = queueRepository;
        }


        /// <summary>
        /// Queue item in the database.
        /// </summary>
        /// <param name="queueModel"></param>
        /// <returns></returns>
        public async Task Post(QueueModel queueModel)
        {
            try
            {
                var translator = new QueueTranslator();
                var queueItem = translator
                    .Translate(queueModel);

                await _queueRepository.QueueItem(queueItem);
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