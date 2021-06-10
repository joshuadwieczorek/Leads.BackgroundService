using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Leads.BackgroundService.Data.Translators.v1;
using Leads.BackgroundService.Repositories;
using Leads.Domain.Models.v1;
using Leads.Library.Validation.v1;

namespace Leads.BackgroundService.Services.v1
{
    public class LeadProviderService : BaseService<LeadProviderService>, ILeadProviderService
    {
        private readonly ILeadProviderRepository _leadProviderRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        /// <param name="leadProviderRepository"></param>
        public LeadProviderService(
              ILogger<LeadProviderService> logger
            , Bugsnag.IClient bugSnag
            , ILeadProviderRepository leadProviderRepository) : base(logger, bugSnag)
        {
            _leadProviderRepository = leadProviderRepository;
        }


        /// <summary>
        /// Create new lead provider.
        /// </summary>
        /// <param name="leadProviderModel"></param>
        /// <returns></returns>
        public async Task<LeadProviderModel> Post(LeadProviderModel leadProviderModel)
        {
            try
            {
                var translator = new LeadProviderTranslator();
                var leadProvider = translator
                    .Translate(leadProviderModel);

                var existingLeadProvider = await _leadProviderRepository.Read(leadProvider.Name);
                if (existingLeadProvider is not null)
                    throw new ValidationException("name", "lead provider already exists by this name");

                leadProvider = await _leadProviderRepository.Create(leadProvider);
                return translator.Translate(leadProvider);
            }
            catch (Exception e)
            {
                bugSnag.Notify(e);
                logger.LogError("{e}", e);
                return null;
            }
        }


        /// <summary>
        /// Get all lead providers.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<LeadProviderModel>> Get()
        {
            try
            {
                var translator = new LeadProviderTranslator();
                var models = await _leadProviderRepository.Read();
                return translator.Translate(models);
            }
            catch (Exception e)
            {
                bugSnag.Notify(e);
                logger.LogError("{e}", e);
                return null;
            }
        }


        /// <summary>
        /// Get lead provider by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<LeadProviderModel> Get(int id)
        {
            try
            {
                var translator = new LeadProviderTranslator();
                var model = await _leadProviderRepository.Read(id);
                if (model is not null)
                    return translator.Translate(model);

                return null;
            }
            catch (Exception e)
            {
                bugSnag.Notify(e);
                logger.LogError("{e}", e);
                return null;
            }
        }


        /// <summary>
        /// Get lead provider by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<LeadProviderModel> Get(string name)
        {
            try
            {
                var translator = new LeadProviderTranslator();
                var model = await _leadProviderRepository.Read(name);
                if (model is not null)
                    return translator.Translate(model);

                return null;
            }
            catch (Exception e)
            {
                bugSnag.Notify(e);
                logger.LogError("{e}", e);
                return null;
            }
        }


        /// <summary>
        /// Delete lead provider.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Delete(int id)
        {
            try
            {
                await _leadProviderRepository.Delete(id);
            }
            catch (Exception e)
            {
                bugSnag.Notify(e);
                logger.LogError("{e}", e);
            }
        }
    }
}