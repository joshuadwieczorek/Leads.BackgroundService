using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Leads.BackgroundService.Repositories;
using Leads.Library.Helpers.v1;
using Leads.Library.Utilities.v1;
using Leads.Domain.Models.v1;
using Leads.Domain.Contracts.v1;
using Leads.BackgroundService.Data.Models;
using AAG.Global.Security;
using AAG.Global.ExtensionMethods;

namespace Leads.BackgroundService.DeliveryActions.v1
{
    internal class AdfDeliveryAction : BaseDeliveryAction<AdfDeliveryAction>, IAdfDeliveryAction
    {
        private readonly CryptographyProvider _cryptographyProvider;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IEmailLogRepository _emailLogRepository;
        private readonly IAdfRepository _adfRepository;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        public AdfDeliveryAction(
              ILogger<AdfDeliveryAction> logger
            , CryptographyProvider cryptographyProvider
            , EmailConfiguration emailConfiguration
            , Bugsnag.IClient bugSnag
            , IQueueRepository queueRepository
            , IEmailLogRepository emailLogRepository
            , IAdfRepository adfRepository) : base(logger, bugSnag, queueRepository)
        {
            _cryptographyProvider = cryptographyProvider;
            _emailConfiguration = emailConfiguration;
            _emailLogRepository = emailLogRepository;
            _adfRepository = adfRepository;
        }


        /// <summary>
        /// Deliver.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="queueItemSource"></param>
        /// <returns></returns>
        public async Task Deliver(Queue queue, QueueModel queueItemSource)
        {
            try
            {
                if (queueItemSource is null)
                    throw new ArgumentNullException(nameof(queueItemSource));

                var queueItem = queueItemSource.Clone();

                var tokenEmailConfiguration = queueItem
                    ?.Token
                    ?.Configuration
                    ?.EmailConfiguration;

                if (tokenEmailConfiguration is null)
                    throw new ArgumentNullException(nameof(tokenEmailConfiguration));

                if (queueItem.LeadInformation is null)
                    throw new ArgumentNullException(nameof(queueItem.LeadInformation));

                var emailSettings = queueItem
                    ?.Token
                    ?.Configuration
                    ?.EmailSettings
                    ?[Domain.Enums.DeliveryAction.Adf];

                if (emailSettings is null || emailSettings?.Template is null)
                    throw new ArgumentNullException("Email settings or email template is null");

                var adfSettings = queueItem
                    ?.Token
                    ?.Configuration
                    ?.AdfSettings;

                if (adfSettings is null)
                    throw new ArgumentNullException(nameof(adfSettings));

                string leadProviderName = queueItem
                    ?.Token
                    ?.LeadProvider
                    ?.Name;

                if (leadProviderName is null)
                    throw new ArgumentNullException("Lead provider name is null");

                emailSettings.Body = TemplatePopulator.Adf(queueItem?.LeadInformation, adfSettings, leadProviderName);
                var validEmailConifiguration = EmailConfigurationHelper.Merge(tokenEmailConfiguration, _emailConfiguration);
                EmailUtility emailUtility = new(validEmailConifiguration);
                await emailUtility.SendAsync(emailSettings);

                Adf adf = new()
                {
                    AdfId = Guid.NewGuid(),
                    AdfXml = TemplatePopulator.Adf(_cryptographyProvider, queueItem?.LeadInformation, adfSettings, leadProviderName, true).Replace("<?adf version \"1.0\"?>", "")
                };

                EmailLog emailLog = new()
                {
                    AdfId = adf.AdfId,
                    Body = _cryptographyProvider.Encrypt(emailSettings.Body),
                    SentAt = DateTime.Now,
                    From = _cryptographyProvider.Encrypt($"{emailSettings.FromName} <{emailSettings.FromEmail}>"),
                    Recipients = _cryptographyProvider.Encrypt(string.Join(',', emailSettings.Recipients)),
                    Subject = emailSettings.Subject
                };              

                await _adfRepository.Create(adf);
                await _emailLogRepository.Create(emailLog);
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