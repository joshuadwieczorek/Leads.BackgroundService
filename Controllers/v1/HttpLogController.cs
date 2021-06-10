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
    public class HttpLogController : ControllerBase
    {
        private readonly ILogger<HttpLogController> _logger;
        private readonly Bugsnag.IClient _bugSnag;
        private readonly IHttpLogService _httpLogService;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        /// <param name="leadProviderService"></param>
        public HttpLogController(
              ILogger<HttpLogController> logger
            , Bugsnag.IClient bugSnag
            , IHttpLogService httpLogService)
        {
            _logger = logger;
            _bugSnag = bugSnag;
            _httpLogService = httpLogService;
        }


        /// <summary>
        /// Create http log entry.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(HttpLogModel httpLogModel)
        {
            try
            {
                var httLog = await _httpLogService.Post(httpLogModel);
                return StatusCode(201, new ApiWrapper<HttpLogModel>(httLog));
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
        /// Update http log entry.
        /// </summary>
        /// <returns></returns>
        [HttpPut("{logId:int}")]
        public async Task<IActionResult> Put(int logId, [FromBody] HttpLogModel httpLogModel)
        {
            try
            {
                await _httpLogService.Put(logId, httpLogModel);
                return NoContent();
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