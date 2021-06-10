using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Leads.BackgroundService.Services.v1;
using Leads.Domain.Contracts.v1;
using Leads.Domain.Models.v1;
using Leads.Library.Validation.v1;

namespace Leads.BackgroundService.Controllers.v1
{
    [ApiController]
    [Route("v1/[controller]")]
    public class LeadProvidersController : ControllerBase
    {
        private readonly ILogger<LeadProvidersController> _logger;
        private readonly Bugsnag.IClient _bugSnag;
        private readonly ILeadProviderService _leadProviderService;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        /// <param name="leadProviderService"></param>
        public LeadProvidersController(
              ILogger<LeadProvidersController> logger
            , Bugsnag.IClient bugSnag
            , ILeadProviderService leadProviderService)
        {
            _logger = logger;
            _bugSnag = bugSnag;
            _leadProviderService = leadProviderService;
        }


        /// <summary>
        /// Create lead provider.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(LeadProviderModel model)
        {
            try
            {
                var leadProvider = await _leadProviderService.Post(model);
                return Ok(new ApiWrapper<LeadProviderModel>(leadProvider));
            }
            catch (ValidationException e)
            {
                return StatusCode(422, new ApiWrapper(e.Errors, status: 422));
            }
            catch (Exception e)
            {
                _bugSnag.Notify(e);
                _logger.LogError("{e}", e);
                return StatusCode(500, new ApiWrapper(e.Message));
            }
        }


        /// <summary>
        /// Get all lead providers.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var leadProviders = await _leadProviderService.Get();
                return Ok(new ApiWrapper<IEnumerable<LeadProviderModel>>(leadProviders));
            }
            catch (Exception e)
            {
                _bugSnag.Notify(e);
                _logger.LogError("{e}", e);
                return StatusCode(500, new ApiWrapper(e.Message));
            }
        }


        /// <summary>
        /// Get lead provider by id.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var leadProvider = await _leadProviderService.Get(id);
                if (leadProvider is not null)
                    return Ok(new ApiWrapper<LeadProviderModel>(leadProvider));
                return NoContent();
            }
            catch (Exception e)
            {
                _bugSnag.Notify(e);
                _logger.LogError("{e}", e);
                return StatusCode(500, new ApiWrapper(e.Message));
            }
        }


        /// <summary>
        /// Get lead provider by name.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name)
        {
            try
            {
                var leadProvider = await _leadProviderService.Get(name);
                if (leadProvider is not null)
                    return Ok(new ApiWrapper<LeadProviderModel>(leadProvider));
                return NoContent();
            }
            catch (Exception e)
            {
                _bugSnag.Notify(e);
                _logger.LogError("{e}", e);
                return StatusCode(500, new ApiWrapper(e.Message));
            }
        }


        /// <summary>
        /// Delete lead provider by id.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _leadProviderService.Delete(id);
                return NoContent();
            }
            catch (Exception e)
            {
                _bugSnag.Notify(e);
                _logger.LogError("{e}", e);
                return StatusCode(500, new ApiWrapper(e.Message));
            }
        }
    }
}