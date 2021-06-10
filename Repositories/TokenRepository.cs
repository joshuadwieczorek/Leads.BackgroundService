using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leads.BackgroundService.Data.Models;
using Microsoft.Extensions.Logging;
using Leads.BackgroundService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Leads.BackgroundService.Repositories
{
    public class TokenRepository : BaseRepository<TokenRepository>, ITokenRepository
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        public TokenRepository(
              IConfiguration configuration
            , ILogger<TokenRepository> logger
            , Bugsnag.IClient bugSnag
            , LeadsDbContext dbContext) : base(configuration, logger, bugSnag, dbContext) { }


        /// <summary>
        /// Create new token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Token> Create(Token token)
        {
            return await retryPolicy.ExecuteAsync<Token>(async () => 
            {
                Guid tokenId = Guid.NewGuid();
                token.TokenId = tokenId;
                token.CreatedAt = DateTime.Now;
                token.CreatedBy = Environment.UserName;
                dbContext.Tokens.Add(token);
                await dbContext.SaveChangesAsync();
                return await Read(tokenId);
            });
        }


        /// <summary>
        /// Read tokens rooftop and provider.
        /// </summary>
        /// <returns></returns>
        public async Task<Token> Read(int? id, int? providerId)
        {
            return await retryPolicy.ExecuteAsync<Token>(async () =>
            {
                var tokens = await Read(id);
                if (tokens is null || !tokens.Any())
                    return null;

                return tokens
                    .Where(t => t.LeadProviderId == providerId && t.ExpiresAt > DateTime.Now)
                    .FirstOrDefault();
            });
        }


        /// <summary>
        /// Read tokens rooftop.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Token>> Read(int? id)
        {
            return await retryPolicy.ExecuteAsync<IEnumerable<Token>>(async () =>
            {
                return await
                    dbContext
                    .Tokens
                    .Where(t => t.RooftopId == id)
                    .Include(t => t.LeadProvider)
                    .ToListAsync();
            });
        }


        /// <summary>
        /// Read token by tokenId.
        /// </summary>
        /// <returns></returns>
        public async Task<Token> Read(Guid tokenId)
        {
            return await retryPolicy.ExecuteAsync<Token>(async () =>
            {
                return await
                    dbContext
                    .Tokens
                    .Where(t => t.TokenId == tokenId)
                    .Include(t => t.LeadProvider)
                    .FirstOrDefaultAsync();
            });
        }


        /// <summary>
        /// Update token expiration.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        public async Task<Token> Update(
              Guid tokenId
            , DateTime expiresAt)
        {
            return await retryPolicy.ExecuteAsync<Token>(async () =>
            {
                Token token = await Read(tokenId);
                token.ExpiresAt = expiresAt;
                token.UpdatedAt = DateTime.Now;
                token.UpdatedBy = Environment.UserName;
                await dbContext.SaveChangesAsync();
                return await Read(tokenId);
            });
        }


        /// <summary>
        /// Update token configuration.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        public async Task<Token> Update(
              Guid tokenId
            , string configuration)
        {
            return await retryPolicy.ExecuteAsync<Token>(async () =>
            {
                Token token = await Read(tokenId);
                token.Configuration = configuration;
                token.UpdatedAt = DateTime.Now;
                token.UpdatedBy = Environment.UserName;
                await dbContext.SaveChangesAsync();
                return await Read(tokenId);
            });
        }
    }
}