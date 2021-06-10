using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Leads.BackgroundService.Data.Translators.v1;
using Leads.BackgroundService.Repositories;
using Leads.Domain.Models.v1;
using Leads.Library.Validation.v1;
using Leads.BackgroundService.Data.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Leads.BackgroundService.Services.v1
{
    public class TokenService : BaseService<TokenService>, ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenRepository _tokenRepository;
        private readonly ILeadProviderRepository _leadProviderRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        /// <param name="leadProviderRepository"></param>
        public TokenService(
              ILogger<TokenService> logger
            , Bugsnag.IClient bugSnag
            , IConfiguration configuration
            , ITokenRepository tokenRepository
            , ILeadProviderRepository leadProviderRepository) : base(logger, bugSnag)
        {
            _configuration = configuration;
            _tokenRepository = tokenRepository;
            _leadProviderRepository = leadProviderRepository;
        }


        /// <summary>
        /// Create new token.
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <returns></returns>
        public async Task<TokenModel> Post(TokenModel tokenModel)
        {
            try
            {
                if (tokenModel is null)
                    throw new ValidationException("token", "payload is invalid");

                if (tokenModel.LeadProvider is null)
                    throw new ValidationException("leadProvider", "required field");

                if (tokenModel.Configuration is null)
                    throw new ValidationException("configuration", "required field");

                if (tokenModel.RooftopId is null || tokenModel.RooftopId is 0)
                    throw new ValidationException("rooftopId", "required field and cannot be '0'");

                LeadProvider leadProvider;

                if (tokenModel.LeadProvider.LeadProviderId.HasValue)
                    leadProvider = await _leadProviderRepository.Read(tokenModel.LeadProvider.LeadProviderId.Value);
                else
                    leadProvider = await _leadProviderRepository.Read(tokenModel.LeadProvider.Name);

                if (leadProvider is null)
                    throw new ValidationException("leadProvider", "leadProvider does not exist by give value(s)");

                var existingToken = await _tokenRepository.Read(tokenModel.RooftopId, leadProvider.LeadProviderId);
                if (existingToken is not null && !existingToken.IsExpired)
                    throw new ValidationException("token", "an active token already exists for this rooftop for this leadProvider");

                int initialTokenExpirationDays = 0;
                if (!int.TryParse(_configuration["InitialTokenExpirationDays"], out initialTokenExpirationDays))
                    initialTokenExpirationDays = 365;

                var tokenTranslator = new TokenTranslator();
                var newToken = tokenTranslator.Translate(tokenModel);
                newToken.LeadProviderId = leadProvider.LeadProviderId;
                newToken.ExpiresAt = DateTime.Now.AddDays(initialTokenExpirationDays);
                newToken = await _tokenRepository.Create(newToken);

                return tokenTranslator.Translate(newToken);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception e)
            {
                bugSnag.Notify(e);
                logger.LogError("{e}", e);
                throw;
            }
        }


        /// <summary>
        /// Read token by id.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public async Task<TokenModel> Get(Guid tokenId)
        {
            try
            {
                var token = await _tokenRepository.Read(tokenId);

                if (token is not null && !token.IsExpired)
                    return new TokenTranslator()
                        .Translate(token);

                return null;
            }
            catch (Exception e)
            {
                bugSnag.Notify(e);
                logger.LogError("{e}", e);
                throw;
            }
        }


        /// <summary>
        /// Read tokens by rooftop id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TokenModel>> Get(int id)
        {
            try
            {
                var tokens = await _tokenRepository.Read(id);
                return new TokenTranslator()
                    .Translate(tokens);
            }
            catch (Exception e)
            {
                bugSnag.Notify(e);
                logger.LogError("{e}", e);
                throw;
            }
        }


        /// <summary>
        /// Read tokens by rooftop id and provider id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="leadProviderId"></param>
        /// <returns></returns>
        public async Task<TokenModel> Get(int id, int leadProviderId)
        {
            try
            {
                var token = await _tokenRepository.Read(id, leadProviderId);

                if (token is not null)
                    return new TokenTranslator()
                        .Translate(token);

                return null;
            }
            catch (Exception e)
            {
                bugSnag.Notify(e);
                logger.LogError("{e}", e);
                throw;
            }
        }


        /// <summary>
        /// Read tokens by rooftop id and provider name.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="leadProviderName"></param>
        /// <returns></returns>
        public async Task<TokenModel> Get(int id, string leadProviderName)
        {
            try
            {
                var leadProvider = await _leadProviderRepository.Read(leadProviderName);
                if (leadProvider is null)
                    throw new ValidationException("leadProviderName", "leadProvider does not exist by give value(s)");

                var token = await _tokenRepository.Read(id, leadProvider.LeadProviderId);

                if (token is not null)
                    return new TokenTranslator()
                        .Translate(token);

                return null;
            }
            catch (ValidationException e)
            {
                throw;
            }
            catch (Exception e)
            {
                bugSnag.Notify(e);
                logger.LogError("{e}", e);
                throw;
            }
        }


        /// <summary>
        /// Expire token.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public async Task Put(Guid tokenId)
        {
            try
            {
                var token = await _tokenRepository.Read(tokenId);

                if (token is not null && !token.IsExpired)
                {
                    var expiresAt = DateTime.Today.AddSeconds(-1);
                    await _tokenRepository.Update(tokenId, expiresAt);
                }
            }
            catch (Exception e)
            {
                bugSnag.Notify(e);
                logger.LogError("{e}", e);
                throw;
            }
        }


        /// <summary>
        /// Update token configuration.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public async Task<TokenModel> Put(Guid tokenId, TokenModel tokenModel)
        {
            try
            {
                if (tokenModel is null)
                    throw new ValidationException("token", "payload is invalid");

                if (tokenModel.Configuration is null)
                    throw new ValidationException("configuration", "required field");

                var token = await _tokenRepository.Read(tokenId);

                if (token is null && !token.IsExpired)
                    throw new ValidationException("tokenId", "an active token does not exit by this tokenId");

                var configuration = JsonConvert.SerializeObject(tokenModel.Configuration);
                token = await _tokenRepository.Update(tokenId, configuration);

                return new TokenTranslator()
                    .Translate(token);
            }
            catch (ValidationException e)
            {
                throw;
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