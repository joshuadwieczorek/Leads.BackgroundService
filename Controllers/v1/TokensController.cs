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
    public class TokensController : ControllerBase
    {
        private readonly ILogger<TokensController> _logger;
        private readonly Bugsnag.IClient _bugSnag;
        private readonly ITokenService _tokenService;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        /// <param name="leadProviderService"></param>
        public TokensController(
              ILogger<TokensController> logger
            , Bugsnag.IClient bugSnag
            , ITokenService tokenService)
        {
            _logger = logger;
            _bugSnag = bugSnag;
            _tokenService = tokenService;
        }


        /// <summary>
        /// Create token.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(TokenModel tokenModel)
        {
            try
            {
                var token = await _tokenService.Post(tokenModel);
                return StatusCode(201, new ApiWrapper<TokenModel>(token));
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
        /// Get all tokens by rooftop id.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{rooftopId:int}")]
        public async Task<IActionResult> Get(int rooftopId)
        {
            try
            {
                var tokens = await _tokenService.Get(rooftopId);
                return Ok(new ApiWrapper<IEnumerable<TokenModel>>(tokens));
            }
            catch (Exception e)
            {
                _bugSnag.Notify(e);
                _logger.LogError("{e}", e);
                return StatusCode(500, new ApiWrapper(e.Message));
            }
        }


        /// <summary>
        /// Get token by tokenId.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{tokenId:guid}")]
        public async Task<IActionResult> Get(Guid tokenId)
        {
            try
            {
                var token = await _tokenService.Get(tokenId);
                if (token is not null)
                    return Ok(new ApiWrapper<TokenModel>(token));

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
        /// Get all tokens by rooftop id and provider id.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{rooftopId:int}/{leadProviderId:int}")]
        public async Task<IActionResult> Get(int rooftopId, int leadProviderId)
        {
            try
            {
                var token = await _tokenService.Get(rooftopId, leadProviderId);
                if (token is not null)
                    return Ok(new ApiWrapper<TokenModel>(token));

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
        /// Get all tokens by rooftop id and provider id.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{rooftopId:int}/{leadProviderName}")]
        public async Task<IActionResult> Get(int rooftopId, string leadProviderName)
        {
            try
            {
                var token = await _tokenService.Get(rooftopId, leadProviderName);
                if (token is not null)
                    return Ok(new ApiWrapper<TokenModel>(token));

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


        /// <summary>
        /// Expire token.
        /// </summary>
        /// <returns></returns>
        [HttpPut("{tokenId:guid}")]
        public async Task<IActionResult> Put(Guid tokenId, [FromBody] TokenModel tokenModel)
        {
            try
            {
                var token = await _tokenService.Put(tokenId, tokenModel);
                if (token is not null)
                    return Ok(new ApiWrapper<TokenModel>(token));

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


        /// <summary>
        /// Expire token.
        /// </summary>
        /// <returns></returns>
        [HttpPut("{tokenId:guid}/expire")]
        public async Task<IActionResult> Put(Guid tokenId)
        {
            try
            {
                await _tokenService.Put(tokenId);
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