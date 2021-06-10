using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using Leads.Domain.Contracts.v1;
using Leads.Domain;

namespace Leads.BackgroundService.Controllers.v1
{
    [ApiController]
    [Route("v1/[controller]")]
    public class DeliveryActionsController : ControllerBase
    {
        private readonly ILogger<DeliveryActionsController> _logger;
        private readonly Bugsnag.IClient _bugSnag;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        /// <param name="leadProviderService"></param>
        public DeliveryActionsController(
              ILogger<DeliveryActionsController> logger
            , Bugsnag.IClient bugSnag)
        {
            _logger = logger;
            _bugSnag = bugSnag;
        }


        /// <summary>
        /// Get delivery actions.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var deliveryActions = Enum
                    .GetNames(typeof(Enums.DeliveryAction))
                    .ToList();

                return StatusCode(201, new ApiWrapper<List<String>>(deliveryActions));
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