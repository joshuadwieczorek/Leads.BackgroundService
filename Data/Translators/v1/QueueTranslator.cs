using Leads.BackgroundService.Data.Models;
using Leads.Domain.Models.v1;
using Newtonsoft.Json;

namespace Leads.BackgroundService.Data.Translators.v1
{
    public class QueueTranslator
    {
        /// <summary>
        /// Translate single token model to token.
        /// </summary>
        /// <param name="queueItem"></param>
        /// <returns></returns>
        public Queue Translate(QueueModel queueItem)
            => new Queue
            {
                Token = JsonConvert.SerializeObject(queueItem?.Token),                
                LeadInformation = JsonConvert.SerializeObject(queueItem?.LeadInformation)                
            };


        /// <summary>
        /// Translate single token to token model.
        /// </summary>
        /// <param name="queueItem"></param>
        /// <returns></returns>
        public QueueModel Translate(Queue queueItem)
            => new QueueModel
            {
                Token = JsonConvert.DeserializeObject<TokenModel>(queueItem?.Token),
                LeadInformation = JsonConvert.DeserializeObject<LeadInformationModel>(queueItem?.LeadInformation)
            };
    }
}