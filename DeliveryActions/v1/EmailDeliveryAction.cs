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

namespace Leads.BackgroundService.DeliveryActions.v1
{
    internal class EmailDeliveryAction : BaseDeliveryAction<EmailDeliveryAction>, IEmailDeliveryAction
    {
        private readonly CryptographyProvider _cryptographyProvider;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IEmailLogRepository _emailLogRepository;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        public EmailDeliveryAction(
              ILogger<EmailDeliveryAction> logger
            , CryptographyProvider cryptographyProvider
            , EmailConfiguration emailConfiguration
            , Bugsnag.IClient bugSnag
            , IQueueRepository queueRepository
            , IEmailLogRepository emailLogRepository) : base(logger, bugSnag, queueRepository)
        {
            _cryptographyProvider = cryptographyProvider;
            _emailConfiguration = emailConfiguration;
            _emailLogRepository = emailLogRepository;
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

                var queueItem = queueItemSource;

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
                    ?[Domain.Enums.DeliveryAction.Email];

                if (emailSettings is null || emailSettings?.Template is null)
                    throw new ArgumentNullException("Email settings or email template is null");

                emailSettings.Body = TemplatePopulator.LeadInformation(queueItem?.LeadInformation, emailSettings.Template);
                var validEmailConifiguration = EmailConfigurationHelper.Merge(tokenEmailConfiguration, _emailConfiguration);

                EmailLog emailLog = new()
                {
                    Body = _cryptographyProvider.Encrypt(emailSettings.Body),
                    SentAt = DateTime.Now,
                    From = _cryptographyProvider.Encrypt($"{emailSettings.FromName} <{emailSettings.FromEmail}>"),
                    Recipients = _cryptographyProvider.Encrypt(string.Join(',', emailSettings.Recipients)),
                    Subject = emailSettings.Subject
                };

                EmailUtility emailUtility = new(validEmailConifiguration);
                await emailUtility.SendAsync(emailSettings);

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