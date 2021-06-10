using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Leads.BackgroundService.Data.Models;
using Leads.Domain.Models.v1;
using System;
using Newtonsoft.Json;

namespace Leads.BackgroundService.Data.Translators.v1
{
    public class TokenTranslator
    {
        /// <summary>
        /// Translate tokens to models.
        /// </summary>
        /// <param name="leadProviders"></param>
        /// <returns></returns>
        public IEnumerable<TokenModel> Translate(IEnumerable<Token> tokens)
        {
            var translatedToken = new ConcurrentBag<TokenModel>();

            if (tokens is not null)
                Parallel.ForEach<Token>(tokens, (token) =>
                {
                    var model = Translate(token);
                    if (model is not null)
                        translatedToken.Add(model);
                });

            return translatedToken
                .ToList()
                .OrderBy(t => t.CreatedAt);
        }


        /// <summary>
        /// Translate single token model to token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Token Translate(TokenModel token)
            => new Token
            {
                TokenId = token?.TokenId ?? Guid.NewGuid(),
                RooftopId = token?.RooftopId,
                LeadProviderId = token?.LeadProvider?.LeadProviderId,
                Configuration = token?.Configuration is not null ? JsonConvert.SerializeObject(token?.Configuration) : String.Empty,
                ExpiresAt = token?.ExpiresAt,
                IsRolling = token?.IsRolling ?? false
            };


        /// <summary>
        /// Translate single token to token model.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public TokenModel Translate(Token token)
            => new TokenModel
            {
                TokenId = token?.TokenId,
                RooftopId = token?.RooftopId,
                LeadProvider = token?.LeadProvider is not null ? new LeadProviderTranslator().Translate(token.LeadProvider) : null,
                Configuration = token?.Configuration is not null ? JsonConvert.DeserializeObject<TokenConfigurationModel>(token.Configuration) : null,
                ExpiresAt = token?.ExpiresAt,
                IsRolling = token.IsRolling,
                CreatedBy = token?.CreatedBy,
                CreatedAt = token?.CreatedAt,
                UpdatedBy = token?.UpdatedBy,
                UpdatedAt = token?.UpdatedAt
            };
    }
}