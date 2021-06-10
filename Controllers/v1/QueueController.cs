using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Leads.BackgroundService.Services.v1;
using Leads.Domain.Contracts.v1;
using Leads.Domain.Models.v1;
using Leads.Library.Validation.v1;

namespace Leads.BackgroundService.Controllers.v1
{
    [ApiController]
    [Route("v1/[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly ILogger<QueueController> _logger;
        private readonly Bugsnag.IClient _bugSnag;
        private readonly IQueueService _queueService;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        /// <param name="leadProviderService"></param>
        public QueueController(
              ILogger<QueueController> logger
            , Bugsnag.IClient bugSnag
            , IQueueService queueService)
        {
            _logger = logger;
            _bugSnag = bugSnag;
            _queueService = queueService;
        }


        /// <summary>
        /// Queue lead for processing later.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(QueueModel queueModel)
        {
            try
            {
                await _queueService.Post(queueModel);
                return StatusCode(202, new ApiWrapper<string>("Lead successfully queued", "success", 202));
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
    }
}