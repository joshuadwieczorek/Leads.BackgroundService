using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Leads.BackgroundService.Data.Models;
using Leads.Domain.Models.v1;
using System;

namespace Leads.BackgroundService.Data.Translators.v1
{
    public class LeadProviderTranslator
    {
        /// <summary>
        /// Translate LeadProvider to models.
        /// </summary>
        /// <param name="leadProviders"></param>
        /// <returns></returns>
        public IEnumerable<LeadProviderModel> Translate(IEnumerable<LeadProvider> leadProviders)
        {
            var translatedProviders = new ConcurrentBag<LeadProviderModel>();

            if (leadProviders is not null)
                Parallel.ForEach<LeadProvider>(leadProviders, (leadProvider) =>
                {
                    var model = Translate(leadProvider);
                    if (model is not null)
                        translatedProviders.Add(model);
                });

            return translatedProviders
                .ToList()
                .OrderBy(l => l.LeadProviderId);
        }


        /// <summary>
        /// Translate single lead provider.
        /// </summary>
        /// <param name="leadProvider"></param>
        /// <returns></returns>
        public LeadProviderModel Translate(LeadProvider leadProvider)
            => new LeadProviderModel
            {
                LeadProviderId = leadProvider?.LeadProviderId,
                Name = leadProvider?.Name,
                CreatedBy = leadProvider?.CreatedBy,
                CreatedAt = leadProvider?.CreatedAt,
                UpdatedBy = leadProvider?.UpdatedBy,
                UpdatedAt = leadProvider?.UpdatedAt
            };

        /// <summary>
        /// Translate single lead provider.
        /// </summary>
        /// <param name="leadProvider"></param>
        /// <returns></returns>
        public LeadProvider Translate(LeadProviderModel leadProvider)
            => new LeadProvider
            {
                LeadProviderId = leadProvider?.LeadProviderId,
                Name = leadProvider?.Name,
                CreatedBy = leadProvider?.CreatedBy ?? Environment.UserName,
                CreatedAt = leadProvider?.CreatedAt ?? DateTime.Now,
                UpdatedBy = leadProvider?.UpdatedBy,
                UpdatedAt = leadProvider?.UpdatedAt
            };
    }
}
